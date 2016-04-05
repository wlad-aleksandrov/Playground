using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AuctionServiceWebApi.Models
{
    public class Bid
    {
        [Required]
        public string BidderId { get; set; }
        [Required]
        public int BidAmount { get; set; }
    }
}