using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using WA.AuctionService.Properties;
using WA.Notification;
using WA.Repository;

namespace WA.AuctionService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AuctionService : IAuctionService
    {
        public IRepository Repository { set; get; }
        public INotificationClient NotificationService { set; get; }

        const string CustomTotalHeader = "X-Total-Count";

        public void CloseAuction(string auctionId)
        {
            if (!Repository.CloseAuction(auctionId))
                throw new WebFaultException<string>(string.Format(Resources.Str_ErrAuctionNotFound, auctionId), System.Net.HttpStatusCode.NotFound);
        }

        public Auction GetAuction(string id)
        {
            Auction? auction;
            try
            {
                auction = Repository.GetAuction(id);
            }
            catch (Exception exc)
            {
                throw new WebFaultException<string>(string.Format(Resources.Str_ErrAuctionCannotBeReceived, id, exc.Message), System.Net.HttpStatusCode.InternalServerError);
            }
            if (!auction.HasValue)
                throw new WebFaultException<string>(string.Format(Resources.Str_ErrAuctionNotFound, id), System.Net.HttpStatusCode.NotFound);

            return auction.Value;
        }

        public IList<Auction> GetAuctions() => Repository.GetAuctions();

        public IList<Auction> GetAuctionsPagination(int offset, int limit)
        {
            var auctions = Repository.GetAuctions(offset, limit);

            //set the total number of open auctions to the Custom Header
            WebOperationContext.Current.OutgoingResponse.Headers.Add(CustomTotalHeader, Repository.GetCountOfOpenAuctions().ToString());
            return auctions;
        }

        public void GetNumberOfAuctions()
        {
            //set the total number of open auctions to the Custom Header
            WebOperationContext.Current.OutgoingResponse.Headers.Add(CustomTotalHeader, Repository.GetCountOfOpenAuctions().ToString());
        }


        public void OpenAuction(string name, string description, string imageUrl, int startingPrice, int estimate)
        {
            var newAuctionId = Repository.OpenAuction(name, description, imageUrl, startingPrice, estimate);
            var ctx = WebOperationContext.Current;

            //set code to 201 ("CREATED")
            ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Created;
            // set Location to new resource
            ctx.OutgoingResponse.Location = $"Auctions/{newAuctionId}";
        }

        public bool TryPlaceBid(string auctionId, string bidderId, int bidAmount)
        {
            if (!Repository.AuctionExists(auctionId))
                throw new WebFaultException<string>(string.Format(Resources.Str_ErrAuctionNotFound, auctionId), System.Net.HttpStatusCode.NotFound);
            var auction = Repository.TryPlaceBid(auctionId, bidderId, bidAmount);

            if (auction.HasValue)
            {
                var notificationMessage = new NotificationMessage(auctionId, bidAmount, bidderId);
                NotificationService.Publish(notificationMessage);
            }

            return auction.HasValue;
        }

        public void Options()
        {

        }
    }
}