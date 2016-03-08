using System.Collections.Generic;

namespace WA.Repository
{
    public interface IRepository
    {
        string OpenAuction(string name, string description, string imageUrl, int startingPrice, int estimate);
        bool CloseAuction(string auctionId);
        Auction? TryPlaceBid(string auctionId, string bidderId, int bidAmount);
        Auction? GetAuction(string auctionId);

        bool AuctionExists(string auctionId);

        IList<Auction> GetAuctions();

        IList<Auction> GetAuctions(int offset, int limit);

        long GetCountOfOpenAuctions();
    }
}