using Aleksandrov.BasketRepository;
using Aleksandrov.BasketService.Models;
using System;
using System.Collections.Generic;

namespace BasketService.Models {
    public sealed class BasketModel {
        public string CustomerId { get; }
        public DateTimeOffset Created { get; }
        public DateTimeOffset Modified { get; }
        public decimal Total { get; }
        public int Count { get; }
        public BasketStatus Status { get; }
        public List<Link> _Links { get; }

        public BasketModel AddLink(Link link) {
            _Links.Add(link);
            return this;
        }

        public BasketModel(
            string customerId,
            DateTimeOffset created,
            DateTimeOffset modified,
            decimal total,
            int count,
            BasketStatus status) {
            CustomerId = customerId;
            Created = created;
            Modified = modified;
            Total = total;
            Count = count;
            Status = status;
            _Links = new List<Link>();
        }
    }
}