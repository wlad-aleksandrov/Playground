using System.Collections.Generic;

namespace BasketApiClient.DTO {
    public class StockItemDto {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<LinkDto> _Links { get; set; }
        public string Picture { get; set; }
        public string Instructions { get; set; }
    }
}