using System.Collections.Generic;

namespace BasketApiClient.DTO {
    public sealed class PaginatedCollectionDto<T> {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public LinkDto[] _Links { get; set; }
    }
}
