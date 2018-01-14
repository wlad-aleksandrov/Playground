using Aleksandrov.BasketRepository;
using Aleksandrov.Inventory;
using BasketService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aleksandrov.BasketService.Models {
    public static class Extensions {
        public static BasketModel ToBasketModel(this Basket basket) {
            return new BasketModel(
                basket.CustomerId,
                basket.Created,
                basket.Modified,
                basket.Total,
                basket.Items.Count,
                basket.Status);
        }

        public static BasketItemModel ToBasketItemModel(this BasketItem basketItem) {
            return new BasketItemModel(
                basketItem.Name,
                basketItem.Description,
                basketItem.Price,
                basketItem.Quantity,
                basketItem.Total);
        }

        public static StockItemModel ToStockItemModel(this CatalogueItem catalogueItem) {
            return new StockItemModel(
                catalogueItem.Name,
                catalogueItem.Description,
                catalogueItem.Price,
                catalogueItem.Picture,
                catalogueItem.Instructions);
        }

    }
}
