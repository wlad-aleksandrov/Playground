using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace WA.Repository
{
    public static class Extensions
    {
        public static Auction? ToAuction(this Dictionary<RedisValue, RedisValue> values)
        {
            if (values == null || values.Count == 0)
                return null;

            // required attributes
            var id = values[AuctionFields.Id];
            var name = values[AuctionFields.Name];
            var imageUrl = values[AuctionFields.ImageUrl];
            var startingPrice = values[AuctionFields.StartingPrice];
            var estimate = values[AuctionFields.Estimate];
            var description = values[AuctionFields.Description];

            // optional attributes
            var highestBid = values.ContainsKey(AuctionFields.HighestBid) ? values[AuctionFields.HighestBid] : (int?)null;
            var highestBidder = values.ContainsKey(AuctionFields.HighestBidder) ? (string)values[AuctionFields.HighestBidder] : null;

            return new Auction(id, name, description, imageUrl, (int)startingPrice, (int)estimate, (int)highestBid, highestBidder);
        }

        public static Auction? ToAuction(this string[] results)
        {
            if (results == null || results.Length == 0)
                return null;

            var columns = results.Where((x, i) => i % 2 == 0).ToList();
            var values = results.Where((x, i) => i % 2 != 0).ToList();

            if (columns.Count != values.Count)
                return null;

            var dict = new Dictionary<RedisValue, RedisValue>();
            for (var i = 0; i < columns.Count; i++)
            {
                dict.Add(columns[i], values[i]);
            }

            return dict.ToAuction();
        }
    }
}
