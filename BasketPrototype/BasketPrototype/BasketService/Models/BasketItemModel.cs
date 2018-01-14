using System.Collections.Generic;

namespace Aleksandrov.BasketService.Models {
    public class BasketItemModel {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public List<Link> _Links { get; set; }

        public BasketItemModel AddLinks(Link link) {
            _Links.Add(link);
            return this;
        }

        public BasketItemModel(
            string name,
            string description,
            decimal price,
            int quantity,
            decimal total) {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Total = total;
            _Links = new List<Link>();
        }
    }
}