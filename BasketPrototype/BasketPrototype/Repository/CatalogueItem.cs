namespace Aleksandrov.Inventory {
    /// <summary>
    /// Represents an item in the catalogue
    /// </summary>
    public sealed class CatalogueItem {
        /// <summary>
        /// Unique Id of this item
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Unique name of this item
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of this item
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Instructions
        /// </summary>
        public string Instructions { get; }
        /// <summary>
        /// Picture of this item (TODO - possibly a link)
        /// </summary>
        public string Picture { get; }
        /// <summary>
        /// Price for one item
        /// </summary>
        public decimal Price { get; }

        public CatalogueItem(
            string id,
            string name,
            string description,
            string picture,
            decimal price) {
            Id = id;
            Name = name;
            Description = description;
            Picture = picture;
            Price = price;
        }
    }
}