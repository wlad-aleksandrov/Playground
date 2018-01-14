using System;
using System.Collections.Generic;

namespace Aleksandrov.BasketRepository {
    internal sealed class DummyBasketEntity {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public List<DummyBasketItemEntity> Items { get; set; }
        public decimal Total { get; set; }
        public BasketStatus Status { get; set; }

        public DummyBasketEntity(
            string id,
            string customerId,
            DateTimeOffset created,
            DateTimeOffset modified,
            List<DummyBasketItemEntity> items,
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
