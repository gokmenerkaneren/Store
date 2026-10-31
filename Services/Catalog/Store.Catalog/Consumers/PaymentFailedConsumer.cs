using MassTransit;
using MongoDB.Driver;
using Store.Catalog.Entities;
using Store.Catalog.Settings;
using Store.Shared.Entities;
using Store.Shared.Events;

namespace Store.Catalog.Consumers
{
    /// <summary>
    /// Ödeme başarısız olduğunda sipariş kalemlerinin stoğunu ilk varyanta atomik $inc ile iade eder.
    /// Sipariş kalemi VariantId taşımadığından stoğun düşüldüğü varyant bilinemez.
    /// </summary>
    public class PaymentFailedConsumer : IConsumer<PaymentFailedIntegrationEvent>
    {
        private readonly IMongoCollection<Product> _productCollection;
        private readonly ILogger<PaymentFailedConsumer> _logger;

        public PaymentFailedConsumer(
            IMongoDatabase database,
            IDatabaseSettings databaseSettings,
            ILogger<PaymentFailedConsumer> logger)
        {
            _productCollection = database.GetCollection<Product>(databaseSettings.ProductCollectionName);
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context)
        {
            var message = context.Message;
            var firstVariant = $"{nameof(Product.Variants)}.0";

            foreach (var item in message.Items)
            {
                // Exists filtresi şart: varyantsız üründe $inc, dizide yeni bir eleman oluştururdu.
                var filter = Builders<Product>.Filter.Eq(p => p.Id, item.ProductId)
                    & Builders<Product>.Filter.Exists(firstVariant);

                // UpdateDate, OrderCreatedConsumer'ın iyimser kilit alanıdır; eşzamanlı bir
                // ReplaceOne'ın bu iadeyi ezmemesi için güncellenmelidir.
                var update = Builders<Product>.Update
                    .Inc($"{firstVariant}.{nameof(ProductVariant.StockQuantity)}", item.Quantity)
                    .Set(p => p.UpdateDate, BaseEntity.GetLocalTime());

                var result = await _productCollection.UpdateOneAsync(filter, update, cancellationToken: context.CancellationToken);

                if (result.MatchedCount == 0)
                {
                    _logger.LogError(
                        "Stok iadesi yapılamadı, elle düzeltme gerekli. Sipariş: {OrderId}, Ürün: {ProductId}, Adet: {Quantity}",
                        message.OrderId,
                        item.ProductId,
                        item.Quantity);
                }
            }

            _logger.LogInformation("Sipariş {OrderId} için ödeme başarısız; stok iadesi tamamlandı. Sebep: {Reason}", message.OrderId, message.Reason);
        }
    }
}
