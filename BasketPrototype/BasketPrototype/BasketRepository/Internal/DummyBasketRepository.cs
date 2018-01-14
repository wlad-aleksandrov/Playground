using Aleksandrov.BasketRepository.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Aleksandrov.BasketRepository {
    public sealed class DummyBasketRepository : IBasketRepository {
        private readonly ConcurrentDictionary<string, DummyBasketEntity> _baskets;

        public DummyBasketRepository() {
            _baskets = new ConcurrentDictionary<string, DummyBasketEntity>();
        }

        public void AddItem(string basketId, BasketItem item, DateTimeOffset timestamp) {
            if (_baskets.TryGetValue(basketId, out var basket)) {
                var existingItem = basket.Items.Find(it => it.CatalogueItemId == item.CatalogueItemId);
                if (existingItem == null) {
                    basket.Items.Add(item.ToDummyBasketItemEntity());
                    basket.Modified = timestamp;
                }
                else {
                    throw new ArgumentException(string.Format(Resources.Error_ItemAlreadyInBasket, item.CatalogueItemId, basketId), nameof(basketId));
                }
            }
            else {
                throw new ArgumentException(string.Format(Resources.Error_BasketNotFound, basketId), nameof(basketId));
            }
        }



        public void ClearItems(string basketId, DateTimeOffset timestamp) {
            if (_baskets.TryGetValue(basketId, out var basket)) {
                basket.Items.Clear();
                basket.Modified = timestamp;
            }
            else {
                throw new ArgumentException(string.Format(Resources.Error_BasketNotFound, basketId), nameof(basketId));
            }
        }

        public string Create(string customerId, BasketStatus basketStatus, DateTimeOffset timestamp) {
            var basket = new DummyBasketEntity(
                Guid.NewGuid().ToString(),
                customerId,
                timestamp,
                timestamp,
                new List<DummyBasketItemEntity>(),
                0,
                basketStatus);

            _baskets.TryAdd(basket.Id, basket);
            return basket.Id;
        }

        public bool Remove(string basketId) => _baskets.TryRemove(basketId, out var _);

        public Basket Get(string basketId) {
            return _baskets.TryGetValue(basketId, out var basket) ? basket.ToBasket() : null;
        }

        public void UpdateBasketTotal(string basketId, decimal total, DateTimeOffset timestamp) {
            if (_baskets.TryGetValue(basketId, out var basket)) {
                basket.Total = total;
                basket.Modified = timestamp;

            }
            else {
                throw new ArgumentException(string.Format(Resources.Error_BasketNotFound, basketId), nameof(basketId));
            }
        }

        public void UpdateBasketStatus(string basketId, BasketStatus status, DateTimeOffset timestamp) {
            if (_baskets.TryGetValue(basketId, out var basket)) {
                basket.Status = status;
                basket.Modified = timestamp;
            }
            else {
                throw new ArgumentException(string.Format(Resources.Error_BasketNotFound, basketId), nameof(basketId));
            }
        }

        public void RemoveItem(string basketId, string catalogueItemId, DateTimeOffset timestamp) {
            if (_baskets.TryGetValue(basketId, out var basket)) {
                var existingItem = basket.Items.Find(it => it.CatalogueItemId == catalogueItemId);
                if (existingItem == null) {
                    throw new ArgumentException(string.Format(Resources.Error_ItemNotFoundInBasket, catalogueItemId, basketId), nameof(basketId));
                }
                else {
                    basket.Items.Remove(existingItem);
                    basket.Modified = timestamp;
                }
            }
            else {
                throw new ArgumentException(string.Format(Resources.Error_BasketNotFound, basketId), nameof(basketId));
            }
        }

        public void UpdateItem(string basketId, string catalogueItemId, int quantity, decimal total, DateTimeOffset timestamp) {
            if (_baskets.TryGetValue(basketId, out var basket)) {
                var existingItem = basket.Items.Find(it => it.CatalogueItemId == catalogueItemId);
                if (existingItem == null) {
                    throw new ArgumentException(string.Format(Resources.Error_ItemNotFoundInBasket, catalogueItemId, basketId), nameof(basketId));
                }
                else {
                    existingItem.Quantity = quantity;
                    existingItem.Total = total;
                    basket.Modified = timestamp;

                }
            }
            else {
                throw new ArgumentException(string.Format(Resources.Error_BasketNotFound, basketId), nameof(basketId));
            }
        }

        public IEnumerable<Basket> GetByCustomerId(string customerId) {
            return _baskets.Values
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.Modified)
                .Select(Extensions.ToBasket);
        }

        public IEnumerable<Basket> GetByCustomerId(string customerId, BasketStatus basketStatus) {
            return _baskets.Values
                .Where(b => b.CustomerId == customerId && b.Status == basketStatus)
                .OrderByDescending(b => b.Modified)
                .Select(Extensions.ToBasket);
        }
    }
}