using System.Collections.Generic;

namespace Aleksandrov.BasketService.Models {
    public sealed class PaginatedCollectionResult<T> {
        public List<T> Items { get; }
        public int Page { get; }
        public int TotalPages { get; }
        public int TotalItems { get; }

        public Link[] _Links { get; }

        public PaginatedCollectionResult(List<T> items, int page, int totalPages, int totalItems, Link[] links) {
            Items = items;
            Page = page;
            TotalPages = totalPages;
            TotalItems = totalItems;
            _Links = links;
        }
    }
}