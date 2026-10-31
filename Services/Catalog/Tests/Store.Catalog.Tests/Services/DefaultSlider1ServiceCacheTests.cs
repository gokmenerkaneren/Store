using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Store.Catalog.Dtos.DefaultSlider1Dtos;
using Store.Catalog.Entities;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Services.DefaultSlider1Services;
using Store.Catalog.Tests.TestSupport;
using Xunit;

namespace Store.Catalog.Tests.Services;

public class DefaultSlider1ServiceCacheTests
{
    private const string Prefix = "catalog:sliders:";

    private readonly Mock<ICatalogCacheService> _cache = new();
    private readonly Mock<AutoMapper.IMapper> _mapper = MongoTestHelpers.Mapper();

    private DefaultSlider1Service CreateSut(Mock<IMongoCollection<DefaultSlider1>> sliders)
    {
        var database = MongoTestHelpers.Database();
        database.Register(sliders);
        return new DefaultSlider1Service(_mapper.Object, MongoTestHelpers.Settings(), database.Object, Mock.Of<ILogger<DefaultSlider1Service>>(), _cache.Object);
    }

    [Theory]
    [InlineData(false, "catalog:sliders:all:False")]
    [InlineData(true, "catalog:sliders:all:True")]
    public async Task GetAll_OnHit_ReturnsCachedWithoutQueryingMongo(bool showDeleted, string expectedKey)
    {
        var cached = new List<ResultDefaultSlider1Dto> { new() { Baslik = "Cached" } };
        _cache.SetupHit(cached);
        var sliders = MongoTestHelpers.Collection<DefaultSlider1>();

        var result = await CreateSut(sliders).GetAllAsync(showDeleted);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<List<ResultDefaultSlider1Dto>>(expectedKey);
        sliders.VerifyNoFind();
    }

    [Fact]
    public async Task GetAll_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new List<ResultDefaultSlider1Dto> { new() { Baslik = "Fresh" } };
        _cache.SetupMiss<List<ResultDefaultSlider1Dto>>();
        _mapper.Setup(m => m.Map<List<ResultDefaultSlider1Dto>>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new DefaultSlider1() })).GetAllAsync();

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task GetById_OnHit_ReturnsCachedWithoutQueryingMongo()
    {
        var id = Guid.NewGuid();
        var cached = new GetByIdDefaultSlider1Dto { Baslik = "Cached" };
        _cache.SetupHit<GetByIdDefaultSlider1Dto?>(cached);
        var sliders = MongoTestHelpers.Collection<DefaultSlider1>();

        var result = await CreateSut(sliders).GetByIdAsync(id);

        result.Should().BeSameAs(cached);
        _cache.VerifyRead<GetByIdDefaultSlider1Dto?>($"catalog:sliders:id:{id}");
        sliders.VerifyNoFind();
    }

    [Fact]
    public async Task GetById_OnMiss_QueriesMongoAndMaps()
    {
        var mapped = new GetByIdDefaultSlider1Dto { Baslik = "Fresh" };
        _cache.SetupMiss<GetByIdDefaultSlider1Dto?>();
        _mapper.Setup(m => m.Map<GetByIdDefaultSlider1Dto?>(It.IsAny<object>())).Returns(mapped);

        var result = await CreateSut(MongoTestHelpers.Collection(new[] { new DefaultSlider1() })).GetByIdAsync(Guid.NewGuid());

        result.Should().BeSameAs(mapped);
    }

    [Fact]
    public async Task Create_InvalidatesOnlyAfterInsert()
    {
        var sliders = MongoTestHelpers.Collection<DefaultSlider1>();
        var calls = new List<string>();
        sliders.Setup(c => c.InsertOneAsync(It.IsAny<DefaultSlider1>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .Callback(() => calls.Add("insert")).Returns(Task.CompletedTask);
        _cache.Setup(c => c.RemoveByPrefixAsync(Prefix)).Callback(() => calls.Add("invalidate")).Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<DefaultSlider1>(It.IsAny<object>())).Returns(new DefaultSlider1());

        await CreateSut(sliders).CreateAsync(new CreateDefaultSlider1Dto());

        calls.Should().Equal("insert", "invalidate");
    }

    [Fact]
    public async Task Create_WhenInsertFails_DoesNotInvalidate()
    {
        var sliders = MongoTestHelpers.Collection<DefaultSlider1>();
        sliders.Setup(c => c.InsertOneAsync(It.IsAny<DefaultSlider1>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("mongo down"));
        _mapper.Setup(m => m.Map<DefaultSlider1>(It.IsAny<object>())).Returns(new DefaultSlider1());

        var act = () => CreateSut(sliders).CreateAsync(new CreateDefaultSlider1Dto());

        await act.Should().ThrowAsync<TimeoutException>();
        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Update_Invalidates()
    {
        var id = Guid.NewGuid();

        await CreateSut(MongoTestHelpers.Collection(new[] { new DefaultSlider1 { Id = id } })).UpdateAsync(new UpdateDefaultSlider1Dto { Id = id });

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Update_WhenSliderMissing_DoesNotInvalidate()
    {
        await CreateSut(MongoTestHelpers.Collection<DefaultSlider1>()).UpdateAsync(new UpdateDefaultSlider1Dto { Id = Guid.NewGuid() });

        _cache.VerifyNotInvalidated();
    }

    [Fact]
    public async Task Delete_Invalidates()
    {
        await CreateSut(MongoTestHelpers.Collection<DefaultSlider1>()).DeleteAsync(Guid.NewGuid());

        _cache.VerifyInvalidated(Prefix);
    }

    [Fact]
    public async Task Restore_Invalidates()
    {
        await CreateSut(MongoTestHelpers.Collection<DefaultSlider1>()).RestoreAsync(Guid.NewGuid());

        _cache.VerifyInvalidated(Prefix);
    }
}
