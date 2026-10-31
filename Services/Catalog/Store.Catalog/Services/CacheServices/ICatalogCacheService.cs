namespace Store.Catalog.Services.CacheServices
{
    /// <summary>
    /// Catalog okumaları için Redis önbelleği. Önbellek hataları çağırana yansıtılmaz:
    /// Redis erişilemezse okumalar veri kaynağına düşer, yazma ve silmeler uyarı loglayıp devam eder.
    /// </summary>
    public interface ICatalogCacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Önbellekteki değeri döner; yoksa factory'yi çalıştırıp sonucu önbelleğe yazar.
        /// Factory'nin fırlattığı hatalar çağırana iletilir. Null sonuçlar önbelleğe yazılmaz.
        /// </summary>
        Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);

        Task RemoveAsync(string key);

        Task RemoveByPrefixAsync(string prefix);
    }
}
