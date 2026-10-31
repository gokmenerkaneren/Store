using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Store.Catalog.Dtos.BrandDtos;
using Store.Catalog.Entities;
using Store.Catalog.Exceptions;
using Store.Catalog.Services.BrandServices;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Tests.TestSupport;
using Xunit;

namespace Store.Catalog.Tests.Services;

public class BrandServiceCacheTests
{
    private const string Prefix = "catalog:brands:";

    private readonly Mock<ICatalogCacheService> _cache = new();
    private readonly Mock<AutoMapper.IMapper> _mapper = MongoTestHelpers.Mapper();

    private BrandService CreateSut(Mock<IMongoCollection<Brand>> brands, Mock<IMongoCollection<Product>>? products = null)
    {
        var database = MongoTestHelpers.Database();
        database.Register(brands);
        database.Register(products ?? MongoTestHelpers.Collection<Product>());
        return new BrandService(_mapper.Object, MongoTestHelpers.Settings(), database.Object, Mock.Of<ILogger<BrandService>>(), _cache.Object);
    }

    [Theory]
    [InlineData(false, "catalog:brands:all:False")]
    [InlineData(true, "catalog:brands:all:True")]
    public async Task GetAll_OnHit_ReturnsCachedWithoutQueryingMongo(bool showDeleted, string expectedKey)
    {
        var cached = new List<ResultBrandDto> { new() { Name = "Cached" } };
        _cache.SetupHit(cached);
        var brands = MongoTestHelpers.Collection<Brand>();

        var result = await CreateSut(brands).GetAllBrandAsync(showDeleted);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<List<ResultBrandDto>>(expectedKey);
        brands.VerifyNoFind();
    }

    [Fact]
    public async Task GetAll_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new List<ResultBrandDto> { new() { Name = "Fresh" } };
        _cache.SetupMiss<List<ResultBrandDto>>();
        _mapper.Setup(m => m.Map<List<ResultBrandDto>>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new Brand() })).GetAllBrandAsync();

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task GetById_OnHit_ReturnsCachedWithoutQueryingMongo()
    {
        var id = Guid.NewGuid();
        var cached = new GetByIdBrandDto { Name = "Cached" };
        _cache.SetupHit<GetByIdBrandDto?>(cached);
        var brands = MongoTestHelpers.Collection<Brand>();

        var result = await CreateSut(brands).GetByIdBrandAsync(id);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<GetByIdBrandDto?>($"catalog:brands:id:{id}");
        brands.VerifyNoFind();
    }

    [Fact]
    public async Task GetById_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new GetByIdBrandDto { Name = "Fresh" };
        _cache.SetupMiss<GetByIdBrandDto?>();
        _mapper.Setup(m => m.Map<GetByIdBrandDto?>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new Brand() })).GetByIdBrandAsync(Guid.NewGuid());

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task Create_InvalidatesOnlyAfterInsert()
    {
        var brands = MongoTestHelpers.Collection<Brand>();
        var calls = new List<string>();
        brands.Setup(c => c.InsertOneAsync(It.IsAny<Brand>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("insert")).Returns(Task.CompletedTask);
        _cache.Setup(c => c.RemoveByPrefixAsync(Prefix)).Callback(() => calls.Add("invalidate")).Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<Brand>(It.IsAny<object>())).Returns(new Brand());

        await CreateSut(brands).CreateBrandAsync(new CreateBrandDto());

        calls.Should().Equal("insert", "invalidate");
    }

    [Fact]
    public async Task Create_WhenInsertFails_DoesNotInvalidate()
    {
        var brands = MongoTestHelpers.Collection<Brand>();
        brands.Setup(c => c.InsertOneAsync(It.IsAny<Brand>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("mongo down"));
        _mapper.Setup(m => m.Map<Brand>(It.IsAny<object>())).Returns(new Brand());

        var act = () => CreateSut(brands).CreateBrandAsync(new CreateBrandDto());

        await act.Should().ThrowAsync<TimeoutException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Update_Invalidates()
    {
        var id = Guid.NewGuid();
        var brands = MongoTestHelpers.Collection(new[] { new Brand { Id = id, IsActive = true } });

        await CreateSut(brands).UpdateBrandAsync(new UpdateBrandDto { Id = id, IsActive = true });

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Update_WhenBrandMissing_DoesNotInvalidate()
    {
        await CreateSut(MongoTestHelpers.Collection<Brand>()).UpdateBrandAsync(new UpdateBrandDto { Id = Guid.NewGuid() });

        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Update_WhenDeactivationBlockedByActiveProducts_DoesNotInvalidate()
    {
        var id = Guid.NewGuid();
        var products = MongoTestHelpers.Collection<Product>();
        MongoTestHelpers.SetupAny(products, hasMatch: true);
        var brands = MongoTestHelpers.Collection(new[] { new Brand { Id = id, IsActive = true } });

        var act = () => CreateSut(brands, products).UpdateBrandAsync(new UpdateBrandDto { Id = id, IsActive = false });

        await act.Should().ThrowAsync<BusinessException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Delete_Invalidates()
    {
        await CreateSut(MongoTestHelpers.Collection<Brand>()).DeleteBrandAsync(Guid.NewGuid());

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Delete_WhenBlockedByActiveProducts_DoesNotInvalidate()
    {
        var products = MongoTestHelpers.Collection<Product>();
        MongoTestHelpers.SetupAny(products, hasMatch: true);

        var act = () => CreateSut(MongoTestHelpers.Collection<Brand>(), products).DeleteBrandAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Restore_Invalidates()
    {
        await CreateSut(MongoTestHelpers.Collection<Brand>()).RestoreBrandAsync(Guid.NewGuid());

        _cache.VerifyInvalidated(Prefix);
    }
}
