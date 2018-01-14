using System;
using System.Collections.Generic;

namespace Aleksandrov.BasketRepository {
    public sealed class Basket {
        public string Id { get; }
        public string CustomerId { get; }
        public DateTimeOffset Created { get; }
        public DateTimeOffset Modified { get; }
        public IReadOnlyList<BasketItem> Items { get; }
        public decimal Total { get; }
        public BasketStatus Status { get; }

        public Basket(
            string id,
            string customerId,
            DateTimeOffset created,
            DateTimeOffset modified,
            IReadOnlyList<BasketItem> items,
            decimal total,
            BasketStatus status) {
            Id = id;
            CustomerId = customerId;
            Created = created;
            Modified = modified;
            Items = items;
            Total = total;
            Status = status;
        }
    }
}