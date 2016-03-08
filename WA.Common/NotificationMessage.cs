namespace WA.Notification
{
    public class NotificationMessage
    {
        public string AuctionId { set; get; }
        public int HighestBid { set; get; }
        public string Bidder { set; get; }

        // parameterless ctor is necessary for JSON Serializer
        public NotificationMessage() { }

        public NotificationMessage(string auctionId, int highestBid, string bidder)
        {
            AuctionId = auctionId;
            HighestBid = highestBid;
            Bidder = bidder;
        }
    }
}