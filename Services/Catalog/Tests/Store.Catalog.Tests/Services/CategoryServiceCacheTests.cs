using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Store.Catalog.Dtos.CategoryDtos;
using Store.Catalog.Entities;
using Store.Catalog.Exceptions;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Services.CategoryServices;
using Store.Catalog.Tests.TestSupport;
using Xunit;

namespace Store.Catalog.Tests.Services;

public class CategoryServiceCacheTests
{
    private const string Prefix = "catalog:categories:";

    private readonly Mock<ICatalogCacheService> _cache = new();
    private readonly Mock<AutoMapper.IMapper> _mapper = MongoTestHelpers.Mapper();

    private CategoryService CreateSut(Mock<IMongoCollection<Category>> categories, Mock<IMongoCollection<Product>>? products = null)
    {
        var database = MongoTestHelpers.Database();
        database.Register(categories);
        database.Register(products ?? MongoTestHelpers.Collection<Product>());
        return new CategoryService(_mapper.Object, MongoTestHelpers.Settings(), database.Object, Mock.Of<ILogger<CategoryService>>(), _cache.Object);
    }

    [Theory]
    [InlineData(false, "catalog:categories:all:False")]
    [InlineData(true, "catalog:categories:all:True")]
    public async Task GetAll_OnHit_ReturnsCachedWithoutQueryingMongo(bool showDeleted, string expectedKey)
    {
        var cached = new List<ResultCategoryDto> { new() { Name = "Cached" } };
        _cache.SetupHit(cached);
        var categories = MongoTestHelpers.Collection<Category>();

        var result = await CreateSut(categories).GetAllCategoryAsync(showDeleted);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<List<ResultCategoryDto>>(expectedKey);
        categories.VerifyNoFind();
    }

    [Fact]
    public async Task GetAll_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new List<ResultCategoryDto> { new() { Name = "Fresh" } };
        _cache.SetupMiss<List<ResultCategoryDto>>();
        _mapper.Setup(m => m.Map<List<ResultCategoryDto>>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new Category() })).GetAllCategoryAsync();

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task GetById_OnHit_ReturnsCachedWithoutQueryingMongo()
    {
        var id = Guid.NewGuid();
        var cached = new GetByIdCategoryDto { Name = "Cached" };
        _cache.SetupHit<GetByIdCategoryDto?>(cached);
        var categories = MongoTestHelpers.Collection<Category>();

        var result = await CreateSut(categories).GetByIdCategoryAsync(id);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<GetByIdCategoryDto?>($"catalog:categories:id:{id}");
        categories.VerifyNoFind();
    }

    [Fact]
    public async Task GetById_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new GetByIdCategoryDto { Name = "Fresh" };
        _cache.SetupMiss<GetByIdCategoryDto?>();
        _mapper.Setup(m => m.Map<GetByIdCategoryDto?>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new Category() })).GetByIdCategoryAsync(Guid.NewGuid());

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task AddorEdit_InvalidatesOnlyAfterInsert()
    {
        var categories = MongoTestHelpers.Collection<Category>();
        var calls = new List<string>();
        categories.Setup(c => c.InsertOneAsync(It.IsAny<Category>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("insert")).Returns(Task.CompletedTask);
        _cache.Setup(c => c.RemoveByPrefixAsync(Prefix)).Callback(() => calls.Add("invalidate")).Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<Category>(It.IsAny<object>())).Returns(new Category());

        await CreateSut(categories).AddorEditAsync(new CreateCategoryDto { Name = "Yeni", Code = "Y1" });

        calls.Should().Equal("insert", "invalidate");
    }

    [Fact]
    public async Task AddorEdit_WhenDuplicate_DoesNotInvalidate()
    {
        var categories = MongoTestHelpers.Collection(new[] { new Category { Name = "Yeni", Code = "Y1" } });

        var act = () => CreateSut(categories).AddorEditAsync(new CreateCategoryDto { Name = "Yeni", Code = "Y1" });

        await act.Should().ThrowAsync<BusinessException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task AddorEdit_WhenInsertFails_DoesNotInvalidate()
    {
        var categories = MongoTestHelpers.Collection<Category>();
        categories.Setup(c => c.InsertOneAsync(It.IsAny<Category>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("mongo down"));
        _mapper.Setup(m => m.Map<Category>(It.IsAny<object>())).Returns(new Category());

        var act = () => CreateSut(categories).AddorEditAsync(new CreateCategoryDto { Name = "Yeni" });

        await act.Should().ThrowAsync<TimeoutException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Update_InvalidatesAfterReplace()
    {
        var id = Guid.NewGuid();
        var existing = new Category { Id = id, IsActive = true };
        var categories = MongoTestHelpers.Collection(Array.Empty<Category>(), new[] { existing });

        await CreateSut(categories).UpdateCategoryAsync(new UpdateCategoryDto { Id = id, Name = "Yeni", IsActive = true });

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Update_WhenCategoryMissing_DoesNotInvalidate()
    {
        var categories = MongoTestHelpers.Collection(Array.Empty<Category>());

        await CreateSut(categories).UpdateCategoryAsync(new UpdateCategoryDto { Id = Guid.NewGuid(), Name = "Yeni" });

        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Update_WhenDeactivationBlockedByActiveProducts_DoesNotInvalidate()
    {
        var id = Guid.NewGuid();
        var categories = MongoTestHelpers.Collection(Array.Empty<Category>(), new[] { new Category { Id = id, IsActive = true } }, new[] { new Category { Id = id, IsActive = true } });
        var products = MongoTestHelpers.Collection<Product>();
        MongoTestHelpers.SetupAny(products, hasMatch: true);

        var act = () => CreateSut(categories, products).UpdateCategoryAsync(new UpdateCategoryDto { Id = id, Name = "Yeni", IsActive = false });

        await act.Should().ThrowAsync<BusinessException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Delete_Invalidates()
    {
        await CreateSut(MongoTestHelpers.Collection<Category>()).DeleteCategoryAsync(Guid.NewGuid());

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Delete_WhenBlockedByActiveProducts_DoesNotInvalidate()
    {
        var products = MongoTestHelpers.Collection<Product>();
        MongoTestHelpers.SetupAny(products, hasMatch: true);

        var act = () => CreateSut(MongoTestHelpers.Collection<Category>(), products).DeleteCategoryAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Restore_Invalidates()
    {
        var id = Guid.NewGuid();

        await CreateSut(MongoTestHelpers.Collection(new[] { new Category { Id = id } })).RestoreCategoryAsync(id);

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Restore_WhenCategoryMissing_DoesNotInvalidate()
    {
        var act = () => CreateSut(MongoTestHelpers.Collection<Category>()).RestoreCategoryAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
        _cache.VerifyNotInvalidated();
    }
}
