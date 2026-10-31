using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Store.Catalog.Services.CacheServices;
using Store.Catalog.Settings;

namespace Store.Catalog.Tests.TestSupport;

internal static class MongoTestHelpers
{
    /// <summary>
    /// Her Find çağrısında sıradaki sonucu dönen koleksiyon mock'u; son sonuç tekrar eder.
    /// </summary>
    public static Mock<IMongoCollection<T>> Collection<T>(params T[][] findResults)
    {
        var results = new Queue<T[]>(findResults);
        var mock = new Mock<IMongoCollection<T>>();

        mock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var batch = results.Count > 1 ? results.Dequeue() : results.Count == 1 ? results.Peek() : Array.Empty<T>();
                return new ListCursor<T>(batch);
            });

        // AnyAsync sorguları _id projeksiyonu ile çalışır.
        SetupAny(mock, hasMatch: false);
        return mock;
    }

    public static void SetupAny<T>(Mock<IMongoCollection<T>> mock, bool hasMatch)
    {
        mock.Setup(c => c.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, BsonDocument>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new ListCursor<BsonDocument>(hasMatch ? new[] { new BsonDocument() } : Array.Empty<BsonDocument>()));
    }

    public static Mock<IMongoDatabase> Database() => new();

    public static void Register<T>(this Mock<IMongoDatabase> database, Mock<IMongoCollection<T>> collection)
    {
        database.Setup(d => d.GetCollection<T>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
            .Returns(collection.Object);
    }

    public static DatabaseSettings Settings() => new();

    /// <summary>Önbellek kaçırma senaryosu: factory gerçekten çalışır.</summary>
    public static void SetupMiss<T>(this Mock<ICatalogCacheService> cache)
    {
        cache.Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<T>>>(), It.IsAny<TimeSpan?>()))
            .Returns((string _, Func<Task<T>> factory, TimeSpan? _) => RunFactory(factory));
    }

    /// <summary>Önbellek isabeti senaryosu: factory hiç çalışmaz.</summary>
    public static void SetupHit<T>(this Mock<ICatalogCacheService> cache, T value)
    {
        cache.Setup(c => c.GetOrSetAsync(It.IsAny<string>(), It.IsAny<Func<Task<T>>>(), It.IsAny<TimeSpan?>()))
            .ReturnsAsync(value);
    }

    public static void VerifyRead<T>(this Mock<ICatalogCacheService> cache, string key)
    {
        cache.Verify(c => c.GetOrSetAsync(key, It.IsAny<Func<Task<T>>>(), TimeSpan.FromHours(1)), Times.Once);
    }

    public static void VerifyInvalidated(this Mock<ICatalogCacheService> cache, string prefix)
    {
        cache.Verify(c => c.RemoveByPrefixAsync(prefix), Times.Once);
    }

    public static void VerifyNotInvalidated(this Mock<ICatalogCacheService> cache)
    {
        cache.Verify(c => c.RemoveByPrefixAsync(It.IsAny<string>()), Times.Never);
    }

    public static void VerifyNoFind<T>(this Mock<IMongoCollection<T>> collection)
    {
        collection.Verify(c => c.FindAsync(
                It.IsAny<FilterDefinition<T>>(),
                It.IsAny<FindOptions<T, T>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    public static Mock<IMapper> Mapper() => new();

    private static async Task<T?> RunFactory<T>(Func<Task<T>> factory) => await factory();

    private sealed class ListCursor<T> : IAsyncCursor<T>
    {
        private readonly IEnumerable<T> _items;
        private bool _consumed;

        public ListCursor(IEnumerable<T> items) => _items = items;

        public IEnumerable<T> Current => _items;

        public bool MoveNext(CancellationToken cancellationToken = default)
        {
            if (_consumed)
            {
                return false;
            }

            _consumed = true;
            return true;
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(MoveNext(cancellationToken));

        public void Dispose()
        {
        }
    }
}
