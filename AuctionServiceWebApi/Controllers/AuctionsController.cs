using AuctionServiceWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WA.Notification;
using WA.Repository;

namespace AuctionServiceWebApi.Controllers
{
    /// <summary>
    /// Auctions Controller
    /// </summary>
    public class AuctionsController : ApiController
    {
        private IRepository _repository;
        private INotificationClient _notificationService;

        public AuctionsController(IRepository repository, INotificationClient notifcationService)
        {
            _repository = repository;
            _notificationService = notifcationService;
        }

        /// <summary>
        /// Get list of all open auctions
        /// </summary>
        /// <returns>List of open auctions</returns>
        public IList<Auction> GetAuctions() => _repository.GetAuctions();

        /// <summary>
        /// Get list of open auctions using pagination
        /// </summary>
        /// <param name="pageNo">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns></returns>
        public HttpResponseMessage GetAuctions(int pageNo, int pageSize = 10)
        {
            var totalAuctions = _repository.GetCountOfOpenAuctions();
            var pageCount = (int)Math.Ceiling(totalAuctions / (double)pageSize);
            var auctions = _repository.GetAuctions(pageNo - 1, pageSize);
            var response = Request.CreateResponse(HttpStatusCode.OK, auctions);

            //setting headers for paging
            response.Headers.Add("X-Paging-PageNo", pageNo.ToString());
            response.Headers.Add("X-Paging-PageSize", pageSize.ToString());
            response.Headers.Add("X-Paging-PageCount", pageCount.ToString());
            response.Headers.Add("X-Paging-TotalRecordCount", totalAuctions.ToString());
            return response;
        }

        /// <summary>
        /// Returns a number of open auctions
        /// </summary>
        /// <returns>Custom Header X-Paging-TotalRecordCount</returns>
        public HttpResponseMessage HeadAuctions()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("X-Paging-TotalRecordCount", _repository.GetCountOfOpenAuctions().ToString());
            return response;
        }

        /// <summary>
        /// Close an auction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult DeleteAuction(string id)
        {
            if (_repository.CloseAuction(id))
                return Ok();
            return NotFound();
        }

        /// <summary>
        /// Opens a new auction
        /// </summary>
        /// <param name="auction"></param>
        /// <returns></returns>
        public HttpResponseMessage PostAuction(AuctionItem auction)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            var newAuctionId = _repository.OpenAuction(auction.Name, auction.Description, auction.ImageUrl,
                auction.StartingPrice, auction.Estimate);

            var response = Request.CreateResponse(HttpStatusCode.Created);
            // set Location to new resource
            response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = newAuctionId }));
            return response;
        }

        /// <summary>
        /// Retrieves an auction
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult GetAuction(string id)
        {
            Auction? auction;
            try
            {
                auction = _repository.GetAuction(id);
            }
            catch (Exception exc)
            {
                return InternalServerError(exc);
            }
            if (!auction.HasValue)
                return NotFound();

            return Ok(auction.Value);
        }
        /// <summary>
        /// Tries to submit an auction bid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bid">d</param>
        /// <returns></returns>
        public IHttpActionResult PutBid(string id, Bid bid)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_repository.AuctionExists(id))
                return NotFound();
            var auction = _repository.TryPlaceBid(id, bid.BidderId, bid.BidAmount);

            if (auction.HasValue)
            {
                var notificationMessage = new NotificationMessage(id, bid.BidAmount, bid.BidderId);
                _notificationService.Publish(notificationMessage);
            }
            return Ok(auction.HasValue);
        }
    }
}