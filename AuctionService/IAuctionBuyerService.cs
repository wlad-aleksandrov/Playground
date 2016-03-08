using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using WA.Repository;

namespace WA.AuctionService
{
    [ServiceContract]
    public interface IAuctionBuyerService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Auctions", ResponseFormat = WebMessageFormat.Json)]
        IList<Auction> GetAuctions();

        [OperationContract]
        [WebGet(UriTemplate = "/Auctions?offset={offset}&limit={limit}", ResponseFormat = WebMessageFormat.Json)]
        IList<Auction> GetAuctionsPagination(int offset, int limit);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Auctions", Method = "HEAD", ResponseFormat = WebMessageFormat.Json)]
        void GetNumberOfAuctions();


        [OperationContract]
        [WebGet(UriTemplate = "/Auctions/{id}", ResponseFormat = WebMessageFormat.Json)]
        Auction GetAuction(string id);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Auctions/{auctionId}", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool TryPlaceBid(string auctionId, string bidderId, int bidAmount);

        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*", ResponseFormat = WebMessageFormat.Json)]
        void Options();
    }
}
