using System.Collections.Generic;

namespace BasketApiClient.DTO {
    public class BasketItemDto {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
        public List<LinkDto> _Links { get; set; }
    }
}