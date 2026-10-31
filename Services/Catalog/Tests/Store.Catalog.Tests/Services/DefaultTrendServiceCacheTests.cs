using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Store.Catalog.Dtos.DefaultTrendDtos;
using Store.Catalog.Entities;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Services.DefaultTrendServices;
using Store.Catalog.Tests.TestSupport;
using Xunit;

namespace Store.Catalog.Tests.Services;

public class DefaultTrendServiceCacheTests
{
    private const string Prefix = "catalog:trends:";

    private readonly Mock<ICatalogCacheService> _cache = new();
    private readonly Mock<AutoMapper.IMapper> _mapper = MongoTestHelpers.Mapper();

    private DefaultTrendService CreateSut(Mock<IMongoCollection<DefaultTrend>> trends, Mock<IMongoCollection<Product>>? products = null, Mock<IMongoCollection<ProductImage>>? images = null)
    {
        var database = MongoTestHelpers.Database();
        database.Register(trends);
        database.Register(products ?? MongoTestHelpers.Collection<Product>());
        database.Register(images ?? MongoTestHelpers.Collection<ProductImage>());
        return new DefaultTrendService(_mapper.Object, MongoTestHelpers.Settings(), database.Object, _cache.Object);
    }

    [Fact]
    public async Task GetAll_OnHit_ReturnsCachedWithoutQueryingMongo()
    {
        var cached = new List<ResultDefaultTrendDto> { new() { ProductName = "Cached" } };
        _cache.SetupHit(cached);
        var trends = MongoTestHelpers.Collection<DefaultTrend>();

        var result = await CreateSut(trends).GetAllDefaultTrendAsync();

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<List<ResultDefaultTrendDto>>("catalog:trends:all");
        trends.VerifyNoFind();
    }

    [Fact]
    public async Task GetAll_OnMiss_JoinsProductDataAndSortsByOrderIndex()
    {
        var productId = Guid.NewGuid();
        var first = new DefaultTrend { ProductId = Guid.NewGuid(), OrderIndex = 2 };
        var second = new DefaultTrend { ProductId = productId, OrderIndex = 1 };
        _cache.SetupMiss<List<ResultDefaultTrendDto>>();
        _mapper.Setup(m => m.Map<ResultDefaultTrendDto>(first)).Returns(new ResultDefaultTrendDto { OrderIndex = 2 });
        _mapper.Setup(m => m.Map<ResultDefaultTrendDto>(second)).Returns(new ResultDefaultTrendDto { OrderIndex = 1 });
        var products = MongoTestHelpers.Collection(new[] { new Product { Id = productId, Name = "Ürün" } });
        var images = MongoTestHelpers.Collection(new[] { new ProductImage { ProductId = productId, ImageUrl = "a.png" } });

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { first, second }), products, images).GetAllDefaultTrendAsync();

        result.Select(x => x.OrderIndex).Should().Equal(1, 2);
        result[0].ProductName.Should().Be("Ürün");
        result[0].ImageUrl.Should().Be("a.png");
    }

    [Fact]
    public async Task GetById_OnHit_ReturnsCachedWithoutQueryingMongo()
    {
        var id = Guid.NewGuid();
        var cached = new GetByIdDefaultTrendDto { Id = id };
        _cache.SetupHit(cached);
        var trends = MongoTestHelpers.Collection<DefaultTrend>();

        var result = await CreateSut(trends).GetByIdDefaultTrendAsync(id);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<GetByIdDefaultTrendDto>($"catalog:trends:id:{id}");
        trends.VerifyNoFind();
    }

    [Fact]
    public async Task GetById_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new GetByIdDefaultTrendDto { Id = Guid.NewGuid() };
        _cache.SetupMiss<GetByIdDefaultTrendDto>();
        _mapper.Setup(m => m.Map<GetByIdDefaultTrendDto>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new DefaultTrend() })).GetByIdDefaultTrendAsync(Guid.NewGuid());

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task Create_InvalidatesOnlyAfterInsert()
    {
        var trends = MongoTestHelpers.Collection<DefaultTrend>();
        var calls = new List<string>();
        trends.Setup(c => c.InsertOneAsync(It.IsAny<DefaultTrend>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("insert")).Returns(Task.CompletedTask);
        _cache.Setup(c => c.RemoveByPrefixAsync(Prefix)).Callback(() => calls.Add("invalidate")).Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<DefaultTrend>(It.IsAny<object>())).Returns(new DefaultTrend());

        await CreateSut(trends).CreateDefaultTrendAsync(new CreateDefaultTrendDto { ProductId = Guid.NewGuid() });

        calls.Should().Equal("insert", "invalidate");
    }

    [Fact]
    public async Task Create_WhenDuplicateIgnored_DoesNotInvalidate()
    {
        var trends = MongoTestHelpers.Collection(new[] { new DefaultTrend() });

        await CreateSut(trends).CreateDefaultTrendAsync(new CreateDefaultTrendDto { ProductId = Guid.NewGuid() });

        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Update_Invalidates()
    {
        var id = Guid.NewGuid();
        var existing = new DefaultTrend { Id = id };
        var dto = new UpdateDefaultTrendDto { Id = id };
        _mapper.Setup(m => m.Map(dto, existing)).Returns(existing);

        await CreateSut(MongoTestHelpers.Collection(new[] { existing })).UpdateDefaultTrendAsync(dto);

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Update_WhenTrendMissing_DoesNotInvalidate()
    {
        await CreateSut(MongoTestHelpers.Collection<DefaultTrend>()).UpdateDefaultTrendAsync(new UpdateDefaultTrendDto { Id = Guid.NewGuid() });

        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Delete_InvalidatesAfterReordering()
    {
        var remaining = new DefaultTrend { OrderIndex = 5 };

        await CreateSut(MongoTestHelpers.Collection(new[] { remaining })).DeleteDefaultTrendAsync(Guid.NewGuid());

        remaining.OrderIndex.Should().Be(1);
        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Delete_WhenReorderFails_StillInvalidatesBecauseRecordIsGone()
    {
        var trends = MongoTestHelpers.Collection<DefaultTrend>();
        trends.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<DefaultTrend>>(), It.IsAny<FindOptions<DefaultTrend, DefaultTrend>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("mongo down"));

        var act = () => CreateSut(trends).DeleteDefaultTrendAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<TimeoutException>();
        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Delete_WhenDeleteFails_DoesNotInvalidate()
    {
        var trends = MongoTestHelpers.Collection<DefaultTrend>();
        trends.Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<DefaultTrend>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("mongo down"));

        var act = () => CreateSut(trends).DeleteDefaultTrendAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<TimeoutException>();
        _cache.VerifyNotInvalidated();
    }
}
