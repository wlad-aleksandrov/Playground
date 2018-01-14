using System.Collections.Generic;

namespace Aleksandrov.BasketService.Models {
    public class StockItemModel {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<Link> _Links { get; set; }
        public string Picture { get; set; }
        public string Instructions { get; set; }

        public StockItemModel AddLinks(Link link) {
            _Links.Add(link);
            return this;
        }

        public StockItemModel(
            string name,
            string description,
            decimal price,
            string picture,
            string instructions) {
            Name = name;
            Description = description;
            Price = price;
            Picture = picture;
            Instructions = instructions;
            _Links = new List<Link>();
        }
    }
}