using System.ServiceModel;
using System.ServiceModel.Web;

namespace WA.AuctionService
{
    [ServiceContract]
    public interface IAuctionService : IAuctionBuyerService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Auctions", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void OpenAuction(string name, string description, string imageUrl, int startingPrice, int estimate);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/Auctions/{auctionId}", ResponseFormat = WebMessageFormat.Json)]
        void CloseAuction(string auctionId);
    }
}