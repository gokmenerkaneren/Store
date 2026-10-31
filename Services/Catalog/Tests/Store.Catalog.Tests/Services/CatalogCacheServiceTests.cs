using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Store.Catalog.Dtos.CategoryDtos;
using Store.Catalog.Dtos.DefaultTrendDtos;
using Store.Catalog.Services.CacheServices;
using Xunit;

namespace Store.Catalog.Tests.Services;

public class CatalogCacheServiceTests
{
    private const string Key = "catalog:test:key";
    private const string Prefix = "catalog:test:";

    private readonly Mock<IDistributedCache> _cache = new();
    private readonly Mock<IConnectionMultiplexer> _multiplexer = new();
    private readonly Mock<IDatabase> _database = new();
    private readonly Mock<ILogger<CatalogCacheService>> _logger = new();
    private readonly CatalogCacheService _sut;

    public CatalogCacheServiceTests()
    {
        _multiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_database.Object);
        _sut = new CatalogCacheService(_cache.Object, _multiplexer.Object, _logger.Object);
    }

    public static TheoryData<Exception> RedisFailures => new()
    {
        new RedisConnectionException(ConnectionFailureType.UnableToConnect, "down"),
        new RedisTimeoutException("slow", CommandStatus.Unknown),
        new RedisServerException("READONLY"),
        new SocketException(),
        new TimeoutException(),
        new ObjectDisposedException("multiplexer")
    };

    // ---------- Get ----------

    [Fact]
    public async Task GetAsync_OnHit_DeserializesCamelCasePayload()
    {
        SetupCached("""{"name":"Elektronik","code":"E1"}""");

        var result = await _sut.GetAsync<ResultCategoryDto>(Key);

        result!.Name.Should().Be("Elektronik");
        result.Code.Should().Be("E1");
    }

    [Fact]
    public async Task GetAsync_OnHit_ReadsPascalCasePayloadCaseInsensitively()
    {
        SetupCached("""{"Name":"Elektronik"}""");

        var result = await _sut.GetAsync<ResultCategoryDto>(Key);

        result!.Name.Should().Be("Elektronik");
    }

    [Fact]
    public async Task GetAsync_OnMiss_ReturnsDefault()
    {
        var result = await _sut.GetAsync<ResultCategoryDto>(Key);

        result.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(RedisFailures))]
    public async Task GetAsync_WhenRedisFails_ReturnsDefaultAndLogsWarning(Exception failure)
    {
        _cache.Setup(c => c.GetAsync(Key, It.IsAny<CancellationToken>())).ThrowsAsync(failure);

        var result = await _sut.GetAsync<ResultCategoryDto>(Key);

        result.Should().BeNull();
        VerifyWarningLogged();
    }

    [Fact]
    public async Task GetAsync_WhenPayloadIsCorrupt_ReturnsDefaultAndLogsWarning()
    {
        SetupCached("{not-json");

        var result = await _sut.GetAsync<ResultCategoryDto>(Key);

        result.Should().BeNull();
        VerifyWarningLogged();
    }

    // ---------- Set ----------

    [Fact]
    public async Task SetAsync_WritesCamelCaseJsonWithRequestedExpiration()
    {
        var expiration = TimeSpan.FromMinutes(5);
        byte[]? payload = null;
        DistributedCacheEntryOptions? options = null;
        CaptureSet((p, o) => { payload = p; options = o; });

        await _sut.SetAsync(Key, new ResultCategoryDto { Name = "Elektronik" }, expiration);

        Encoding.UTF8.GetString(payload!).Should().Contain("\"name\":\"Elektronik\"");
        options!.AbsoluteExpirationRelativeToNow.Should().Be(expiration);
    }

    [Fact]
    public async Task SetAsync_WithoutExpiration_UsesDefaultSoEntriesNeverLiveForever()
    {
        DistributedCacheEntryOptions? options = null;
        CaptureSet((_, o) => options = o);

        await _sut.SetAsync(Key, new ResultCategoryDto());

        options!.AbsoluteExpirationRelativeToNow.Should().Be(CatalogCacheService.DefaultExpiration);
    }

    [Fact]
    public async Task SetAsync_WithNullValue_DoesNotWrite()
    {
        await _sut.SetAsync<ResultCategoryDto?>(Key, null);

        VerifyNeverWritten();
    }

    [Theory]
    [MemberData(nameof(RedisFailures))]
    public async Task SetAsync_WhenRedisFails_DoesNotThrow(Exception failure)
    {
        _cache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);

        var act = () => _sut.SetAsync(Key, new ResultCategoryDto());

        await act.Should().NotThrowAsync();
        VerifyWarningLogged();
    }

    // ---------- GetOrSet ----------

    [Fact]
    public async Task GetOrSetAsync_OnHit_ReturnsCachedAndSkipsFactoryAndWrite()
    {
        SetupCached("""{"name":"Cached"}""");
        var factoryCalls = 0;

        var result = await _sut.GetOrSetAsync(Key, () =>
        {
            factoryCalls++;
            return Task.FromResult(new ResultCategoryDto { Name = "Fresh" });
        });

        result!.Name.Should().Be("Cached");
        factoryCalls.Should().Be(0);
        VerifyNeverWritten();
    }

    [Fact]
    public async Task GetOrSetAsync_OnHit_TreatsCachedDefaultValueAsHit()
    {
        SetupCached("0");
        var factoryCalls = 0;

        var result = await _sut.GetOrSetAsync(Key, () =>
        {
            factoryCalls++;
            return Task.FromResult(42);
        });

        result.Should().Be(0);
        factoryCalls.Should().Be(0);
    }

    [Fact]
    public async Task GetOrSetAsync_OnMiss_RunsFactoryAndStoresResult()
    {
        byte[]? payload = null;
        CaptureSet((p, _) => payload = p);

        var result = await _sut.GetOrSetAsync(Key, () => Task.FromResult(new ResultCategoryDto { Name = "Fresh" }));

        result!.Name.Should().Be("Fresh");
        Encoding.UTF8.GetString(payload!).Should().Contain("\"name\":\"Fresh\"");
    }

    [Fact]
    public async Task GetOrSetAsync_WhenFactoryReturnsNull_DoesNotCacheIt()
    {
        var result = await _sut.GetOrSetAsync<ResultCategoryDto?>(Key, () => Task.FromResult<ResultCategoryDto?>(null));

        result.Should().BeNull();
        VerifyNeverWritten();
    }

    [Theory]
    [MemberData(nameof(RedisFailures))]
    public async Task GetOrSetAsync_WhenReadAndWriteFail_ReturnsFactoryResultOnce(Exception failure)
    {
        _cache.Setup(c => c.GetAsync(Key, It.IsAny<CancellationToken>())).ThrowsAsync(failure);
        _cache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(failure);
        var factoryCalls = 0;

        var result = await _sut.GetOrSetAsync(Key, () =>
        {
            factoryCalls++;
            return Task.FromResult(new ResultCategoryDto { Name = "Fresh" });
        });

        result!.Name.Should().Be("Fresh");
        factoryCalls.Should().Be(1);
    }

    [Fact]
    public async Task GetOrSetAsync_WhenPayloadIsCorrupt_FallsBackToFactoryAndRewrites()
    {
        SetupCached("{not-json");

        var result = await _sut.GetOrSetAsync(Key, () => Task.FromResult(new ResultCategoryDto { Name = "Fresh" }));

        result!.Name.Should().Be("Fresh");
        _cache.Verify(c => c.SetAsync(Key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrSetAsync_WhenFactoryThrows_PropagatesAndDoesNotWrite()
    {
        var act = () => _sut.GetOrSetAsync<ResultCategoryDto>(Key, () => throw new InvalidOperationException("mongo down"));

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("mongo down");
        VerifyNeverWritten();
    }

    [Fact]
    public async Task GetOrSetAsync_RoundTripsRealDtos()
    {
        byte[]? payload = null;
        CaptureSet((p, _) => payload = p);
        var original = new List<ResultDefaultTrendDto>
        {
            new() { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), OrderIndex = 2, IsActive = true, ProductName = "Ürün", Price = 12.5m, ImageUrl = "a.png" }
        };
        await _sut.SetAsync(Key, original);
        _cache.Setup(c => c.GetAsync(Key, It.IsAny<CancellationToken>())).ReturnsAsync(payload);

        var result = await _sut.GetAsync<List<ResultDefaultTrendDto>>(Key);

        result.Should().BeEquivalentTo(original);
    }

    [Fact]
    public async Task GetAsync_RoundTripsBaseEntityAuditFields()
    {
        byte[]? payload = null;
        CaptureSet((p, _) => payload = p);
        var original = new GetByIdCategoryDto
        {
            Name = "Elektronik",
            ParentId = Guid.NewGuid(),
            IsActive = false,
            UpdateDate = new DateTimeOffset(2026, 11, 1, 10, 0, 0, TimeSpan.FromHours(3))
        };
        await _sut.SetAsync(Key, original);
        _cache.Setup(c => c.GetAsync(Key, It.IsAny<CancellationToken>())).ReturnsAsync(payload);

        var result = await _sut.GetAsync<GetByIdCategoryDto>(Key);

        result.Should().BeEquivalentTo(original);
    }

    // ---------- Remove ----------

    [Fact]
    public async Task RemoveAsync_DelegatesToDistributedCache()
    {
        await _sut.RemoveAsync(Key);

        _cache.Verify(c => c.RemoveAsync(Key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(RedisFailures))]
    public async Task RemoveAsync_WhenRedisFails_DoesNotThrow(Exception failure)
    {
        _cache.Setup(c => c.RemoveAsync(Key, It.IsAny<CancellationToken>())).ThrowsAsync(failure);

        var act = () => _sut.RemoveAsync(Key);

        await act.Should().NotThrowAsync();
        VerifyWarningLogged();
    }

    // ---------- RemoveByPrefix ----------

    [Fact]
    public async Task RemoveByPrefixAsync_ScansWithInstanceNameAndDeletesRawKeys()
    {
        var realKeys = new[] { $"CatalogCache_{Prefix}a", $"CatalogCache_{Prefix}b" };
        var server = AddServer(new IPEndPoint(IPAddress.Loopback, 6379), realKeys);

        await _sut.RemoveByPrefixAsync(Prefix);

        server.Verify(s => s.KeysAsync(
                It.IsAny<int>(),
                It.Is<RedisValue>(p => p == $"CatalogCache_{Prefix}*"),
                It.IsAny<int>(),
                It.IsAny<long>(),
                It.IsAny<int>(),
                It.IsAny<CommandFlags>()),
            Times.Once);
        foreach (var key in realKeys)
        {
            _database.Verify(d => d.KeyDeleteAsync((RedisKey)key, It.IsAny<CommandFlags>()), Times.Once);
        }

        // IDistributedCache anahtarı ikinci kez öneklerdi: CatalogCache_CatalogCache_...
        _cache.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoveByPrefixAsync_CoversEveryEndpoint()
    {
        AddServer(new IPEndPoint(IPAddress.Loopback, 6379), $"CatalogCache_{Prefix}a");
        AddServer(new IPEndPoint(IPAddress.Loopback, 6380), $"CatalogCache_{Prefix}b");

        await _sut.RemoveByPrefixAsync(Prefix);

        _database.Verify(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RemoveByPrefixAsync_SkipsDisconnectedServerAndContinues()
    {
        var down = AddServer(new IPEndPoint(IPAddress.Loopback, 6379), $"CatalogCache_{Prefix}a");
        down.Setup(s => s.IsConnected).Returns(false);
        AddServer(new IPEndPoint(IPAddress.Loopback, 6380), $"CatalogCache_{Prefix}b");

        await _sut.RemoveByPrefixAsync(Prefix);

        VerifyKeysNeverScanned(down);
        _database.Verify(d => d.KeyDeleteAsync((RedisKey)$"CatalogCache_{Prefix}b", It.IsAny<CommandFlags>()), Times.Once);
        VerifyWarningLogged();
    }

    [Fact]
    public async Task RemoveByPrefixAsync_SkipsReplicas()
    {
        var replica = AddServer(new IPEndPoint(IPAddress.Loopback, 6379), $"CatalogCache_{Prefix}a");
        replica.Setup(s => s.IsReplica).Returns(true);

        await _sut.RemoveByPrefixAsync(Prefix);

        VerifyKeysNeverScanned(replica);
    }

    [Fact]
    public async Task RemoveByPrefixAsync_WithNoMatchingKeys_DeletesNothing()
    {
        AddServer(new IPEndPoint(IPAddress.Loopback, 6379));

        await _sut.RemoveByPrefixAsync(Prefix);

        _database.Verify(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(RedisFailures))]
    public async Task RemoveByPrefixAsync_WhenGetEndPointsFails_DoesNotThrow(Exception failure)
    {
        _multiplexer.Setup(m => m.GetEndPoints(It.IsAny<bool>())).Throws(failure);

        var act = () => _sut.RemoveByPrefixAsync(Prefix);

        await act.Should().NotThrowAsync();
        VerifyWarningLogged();
    }

    [Theory]
    [MemberData(nameof(RedisFailures))]
    public async Task RemoveByPrefixAsync_WhenDeleteFails_DoesNotThrow(Exception failure)
    {
        AddServer(new IPEndPoint(IPAddress.Loopback, 6379), $"CatalogCache_{Prefix}a");
        _database.Setup(d => d.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ThrowsAsync(failure);

        var act = () => _sut.RemoveByPrefixAsync(Prefix);

        await act.Should().NotThrowAsync();
        VerifyWarningLogged();
    }

    [Fact]
    public async Task RemoveByPrefixAsync_WhenOneServerFails_StillCleansTheOthers()
    {
        var failing = new IPEndPoint(IPAddress.Loopback, 6379);
        var healthy = new IPEndPoint(IPAddress.Loopback, 6380);
        AddServer(healthy, $"CatalogCache_{Prefix}b");
        _multiplexer.Setup(m => m.GetEndPoints(It.IsAny<bool>())).Returns(new EndPoint[] { failing, healthy });
        _multiplexer.Setup(m => m.GetServer(failing, It.IsAny<object>())).Throws(new RedisConnectionException(ConnectionFailureType.SocketFailure, "down"));

        await _sut.RemoveByPrefixAsync(Prefix);

        _database.Verify(d => d.KeyDeleteAsync((RedisKey)$"CatalogCache_{Prefix}b", It.IsAny<CommandFlags>()), Times.Once);
        VerifyWarningLogged();
    }

    // ---------- Helpers ----------

    private void SetupCached(string json)
    {
        _cache.Setup(c => c.GetAsync(Key, It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes(json));
    }

    private void CaptureSet(Action<byte[], DistributedCacheEntryOptions> capture)
    {
        _cache.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Callback((string _, byte[] payload, DistributedCacheEntryOptions options, CancellationToken _) => capture(payload, options))
            .Returns(Task.CompletedTask);
    }

    private readonly List<EndPoint> _endpoints = new();

    private Mock<IServer> AddServer(EndPoint endpoint, params string[] keys)
    {
        var server = new Mock<IServer>();
        server.Setup(s => s.IsConnected).Returns(true);
        server.Setup(s => s.EndPoint).Returns(endpoint);
        server.Setup(s => s.KeysAsync(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
            .Returns(() => ToAsyncEnumerable(keys));

        _endpoints.Add(endpoint);
        _multiplexer.Setup(m => m.GetEndPoints(It.IsAny<bool>())).Returns(_endpoints.ToArray());
        _multiplexer.Setup(m => m.GetServer(endpoint, It.IsAny<object>())).Returns(server.Object);
        return server;
    }

    private static async IAsyncEnumerable<RedisKey> ToAsyncEnumerable(IEnumerable<string> keys)
    {
        foreach (var key in keys)
        {
            yield return key;
            await Task.Yield();
        }
    }

    private static void VerifyKeysNeverScanned(Mock<IServer> server)
    {
        server.Verify(s => s.KeysAsync(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<long>(), It.IsAny<int>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    private void VerifyNeverWritten()
    {
        _cache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private void VerifyWarningLogged()
    {
        _logger.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
