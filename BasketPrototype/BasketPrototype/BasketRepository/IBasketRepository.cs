using System;
using System.Collections.Generic;

namespace Aleksandrov.BasketRepository {
    public interface IBasketRepository {
        Basket Get(string basketId);
        IEnumerable<Basket> GetByCustomerId(string customerId);
        IEnumerable<Basket> GetByCustomerId(string customerId, BasketStatus basketStatus);

        string Create(string customerId, BasketStatus basketStatus, DateTimeOffset timestamp);

        void UpdateBasketTotal(string basketId, decimal total, DateTimeOffset timestamp);
        void UpdateBasketStatus(string basketId, BasketStatus status, DateTimeOffset timestamp);

        void AddItem(string basketId, BasketItem item, DateTimeOffset timestamp);
        void UpdateItem(string basketId, string catalogueItemId, int quantity, decimal total, DateTimeOffset timestamp);
        void RemoveItem(string basketId, string catalogueItemId, DateTimeOffset timestamp);
        bool Remove(string basketId);
        void ClearItems(string basketId, DateTimeOffset timestamp);
    }
}