using System;
using System.Collections.Generic;

namespace BasketApiClient.DTO {
    public sealed class BasketDto {
        public string CustomerId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
        public BasketStatus Status { get; set; }
        public List<LinkDto> _Links { get; set; }
    }
}
