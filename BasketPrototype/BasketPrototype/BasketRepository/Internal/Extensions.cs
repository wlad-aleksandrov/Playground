using System.Linq;

namespace Aleksandrov.BasketRepository {
    internal static class Extensions {
        internal static Basket ToBasket(this DummyBasketEntity basketEntity) {
            return new Basket(
                basketEntity.Id,
                basketEntity.CustomerId,
                basketEntity.Created,
                basketEntity.Modified,
                basketEntity.Items.Select(ToBasketItem).ToList(),
                basketEntity.Total,
                basketEntity.Status);
        }

        internal static BasketItem ToBasketItem(this DummyBasketItemEntity basketItemEntity) {
            return new BasketItem(
                basketItemEntity.CatalogueItemId,
                basketItemEntity.Name,
                basketItemEntity.Description,
                basketItemEntity.Price,
                basketItemEntity.Quantity,
                basketItemEntity.Total);
        }

        internal static DummyBasketItemEntity ToDummyBasketItemEntity(this BasketItem basketItem) {
            return new DummyBasketItemEntity(
              basketItem.CatalogueItemId,
              basketItem.Name,
              basketItem.Description,
              basketItem.Price,
              basketItem.Quantity,
              basketItem.Total);
        }
    }
}
