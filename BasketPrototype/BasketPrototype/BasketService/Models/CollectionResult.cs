using System.Collections.Generic;

namespace Aleksandrov.BasketService.Models {
    public sealed class CollectionResult<T> {
        public List<T> Items { get; }
        public Link[] _Links { get; }

        public CollectionResult(List<T> items, Link[] links) {
            Items = items;
            _Links = links;
        }
    }
}