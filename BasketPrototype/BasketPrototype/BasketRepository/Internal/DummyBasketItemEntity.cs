namespace Aleksandrov.BasketRepository {
    internal sealed class DummyBasketItemEntity {
        public string CatalogueItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        public DummyBasketItemEntity(
            string catalogueItemId,
            string name,
            string description,
            decimal price,
            int quantity,
            decimal total) {
            CatalogueItemId = catalogueItemId;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            Total = total;
        }
    }
}
