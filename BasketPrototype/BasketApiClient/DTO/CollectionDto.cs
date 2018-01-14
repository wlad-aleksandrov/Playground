using System.Collections.Generic;

namespace BasketApiClient.DTO {
    public sealed class CollectionDto<T> {
        public List<T> Items { get; set; }
        public LinkDto[] _Links { get; set; }
    }
}