namespace Aleksandrov.BasketRepository {
    public sealed class BasketItem {
        public string CatalogueItemId { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int Quantity { get; }
        public decimal Total { get; }

        public BasketItem(
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