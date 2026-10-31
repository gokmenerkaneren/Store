using MassTransit;
using MongoDB.Driver;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Events;

namespace Store.Catalog.Consumers
{
    /// <summary>
    /// Sipariş oluşturuldu olayını tüketir: varyant stoklarını düşer ve sonucu
    /// StockReserved / StockReservationFailed olayı olarak yayınlar.
    /// Sipariş birden fazla ürün dokümanına yazdığı için yazmalar atomik değildir; sonradan çıkan
    /// bir çakışmada önceden düşülen stoklar iade edilir.
    /// </summary>
    public class OrderCreatedConsumer : IConsumer<OrderCreatedIntegrationEvent>
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(
            IMongoDatabase database,
            IDatabaseSettings databaseSettings,
            IPublishEndpoint publishEndpoint,
            ILogger<OrderCreatedConsumer> logger)
        {
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreatedIntegrationEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;

            var productIds = message.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productCollection
                .Find(Builders<Product>.Filter.In(p => p.Id, productIds) & Builders<Product>.Filter.Eq(p => p.IsDeleted, false))
                .ToListAsync(cancellationToken);
            var productsById = products.ToDictionary(p => p.Id);

            // Çakışma denetimi için ürünlerin okunduğu andaki UpdateDate değerleri
            var currentUpdateDates = products.ToDictionary(p => p.Id, p => p.UpdateDate);

            var allItemsAvailable = true;
            string? failureReason = null;
            var reservations = new List<StockReservation>();

            foreach (var item in message.Items)
            {
                if (!productsById.TryGetValue(item.ProductId, out var product))
                {
                    allItemsAvailable = false;
                    failureReason = $"Ürün bulunamadı: {item.ProductId}";
                    break;
                }

                // Sipariş kalemi VariantId taşımadığından, adedi tek başına karşılayan ilk varyant seçilir.
                // Düşüm bellekteki kopyada yapıldığı için aynı ürünün sonraki kalemleri kalan stoğu görür;
                // veritabanına yalnızca tüm kalemler karşılanırsa yazılır.
                var variant = product.Variants.FirstOrDefault(v => v.StockQuantity >= item.Quantity);
                if (variant == null)
                {
                    allItemsAvailable = false;
                    failureReason = $"Yetersiz stok: {item.ProductId} numaralı ürünün hiçbir varyantında {item.Quantity} adet stok yok.";
                    break;
                }

                variant.StockQuantity -= item.Quantity;
                reservations.Add(new StockReservation(product.Id, variant.VariantId, item.Quantity));
            }

            if (allItemsAvailable)
            {
                failureReason = await PersistReservationsAsync(products, currentUpdateDates, reservations, cancellationToken);
            }

            if (failureReason != null)
            {
                _logger.LogWarning("Sipariş {OrderId} için stok ayrılamadı: {Reason}", message.OrderId, failureReason);

                await _publishEndpoint.Publish(
                    new StockReservationFailedIntegrationEvent(message.OrderId, message.UserId, message.TotalPrice, failureReason, DateTime.UtcNow, message.Items),
                    cancellationToken);
                return;
            }

            _logger.LogInformation("Sipariş {OrderId} için stok ayrıldı ({ItemCount} kalem).", message.OrderId, message.Items.Count);

            await _publishEndpoint.Publish(
                new StockReservedIntegrationEvent(message.OrderId, message.UserId, message.TotalPrice, DateTime.UtcNow, message.Items),
                cancellationToken);
        }

        /// <summary>
        /// Ürünleri iyimser eşzamanlılık ile yazar. Çakışma olursa önceki yazmaları geri alır ve sebebi döner.
        /// </summary>
        private async Task<string?> PersistReservationsAsync(
            List<Product> products,
            Dictionary<Guid, DateTimeOffset?> currentUpdateDates,
            List<StockReservation> reservations,
            CancellationToken cancellationToken)
        {
            var persistedProductIds = new HashSet<Guid>();

            try
            {
                foreach (var product in products)
                {
                    var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id)
                        & Builders<Product>.Filter.Eq(p => p.UpdateDate, currentUpdateDates[product.Id]);

                    product.UpdateDate = BaseEntity.GetLocalTime();

                    var result = await _productCollection.ReplaceOneAsync(filter, product, cancellationToken: cancellationToken);
                    if (result.MatchedCount == 0)
                    {
                        await RestoreStockAsync(persistedProductIds, reservations);
                        return $"Eşzamanlılık çakışması: {product.Id} numaralı ürün başka bir işlem tarafından güncellendi.";
                    }

                    persistedProductIds.Add(product.Id);
                }
            }
            catch
            {
                await RestoreStockAsync(persistedProductIds, reservations);
                throw;
            }

            return null;
        }

        /// <summary>
        /// Bu arada başka işlemlerin yaptığı düşümleri ezmemek için stoğu atomik $inc ile iade eder.
        /// </summary>
        private async Task RestoreStockAsync(HashSet<Guid> persistedProductIds, List<StockReservation> reservations)
        {
            foreach (var reservation in reservations.Where(r => persistedProductIds.Contains(r.ProductId)))
            {
                try
                {
                    var filter = Builders<Product>.Filter.Eq(p => p.Id, reservation.ProductId)
                        & Builders<Product>.Filter.ElemMatch(p => p.Variants, v => v.VariantId == reservation.VariantId);
                    var update = Builders<Product>.Update
                        .Inc($"{nameof(Product.Variants)}.$.{nameof(ProductVariant.StockQuantity)}", reservation.Quantity)
                        .Set(p => p.UpdateDate, BaseEntity.GetLocalTime());

                    await _productCollection.UpdateOneAsync(filter, update);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Stok iadesi başarısız, elle düzeltme gerekli. Ürün: {ProductId}, Varyant: {VariantId}, Adet: {Quantity}",
                        reservation.ProductId,
                        reservation.VariantId,
                        reservation.Quantity);
                }
            }
        }

        private sealed record StockReservation(Guid ProductId, Guid VariantId, int Quantity);
    }
}
