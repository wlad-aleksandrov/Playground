using System.Runtime.Serialization;

namespace WA.Repository
{
    [DataContract]
    public struct Auction
    {
        [DataMember]
        public readonly string Id;
        [DataMember]
        public readonly string Name;
        [DataMember]
        public readonly string Description;
        [DataMember]
        public readonly string ImageUrl;
        [DataMember]
        public readonly int StartingPrice;
        [DataMember]
        public readonly int? HighestBid;
        [DataMember]
        public readonly string HighestBidder;
        [DataMember]
        public readonly int Estimate;

        public Auction(string id, string name, string description, string imageUrl, int startingPrice, int estimate,
            int highestBid, string highestBidder)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            StartingPrice = startingPrice;
            HighestBid = highestBid;
            HighestBidder = highestBidder;
            Description = description;
            Estimate = estimate;
        }
    }
}