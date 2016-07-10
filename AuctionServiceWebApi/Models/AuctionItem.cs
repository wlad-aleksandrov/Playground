using System.ComponentModel.DataAnnotations;

namespace AuctionServiceWebApi.Models
{
    public class AuctionItem
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public int StartingPrice { get; set; }
        [Required]
        public int Estimate { get; set; }
    }
}