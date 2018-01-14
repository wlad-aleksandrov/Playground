using System;
using System.Collections.Generic;
using Aleksandrov.BasketRepository;
using BasketService.Models;
using BasketService.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Aleksandrov.BasketService;
using Aleksandrov.BasketService.Models;
using System.Net;
using Aleksandrov.Inventory;

namespace BasketService.Controllers {
    [Route("api/[controller]")]
    public class BasketsController : Controller {

        private readonly IBasketRepository _basketRepository;
        private readonly ICatalogue _catalogue;
        private readonly IClock _clock;

        public BasketsController(IBasketRepository basketRepository, ICatalogue catalogue, IClock clock) {
            _basketRepository = basketRepository;
            _catalogue = catalogue;
            _clock = clock;
        }

        [HttpPost]
        public IActionResult Create([FromBody] NewBasketRequest basketRequest) {
            if (_basketRepository.GetByCustomerId(basketRequest.CustomerId, BasketStatus.Open).Any()) {
                return new StatusCodeResult((int)HttpStatusCode.Conflict);
            }

            var basketId = _basketRepository.Create(basketRequest.CustomerId, BasketStatus.Open, _clock.Now);
            return Created(new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{basketId}"), null);
        }

        [HttpGet]
        public IActionResult GetOpenBasketByCustomerId(string customerId) {
            var basket = _basketRepository.GetByCustomerId(customerId, BasketStatus.Open).ToList();
            if (basket.Count == 0) {
                return Ok();
            }

            if (basket.Count > 1) {
                return new StatusCodeResult((int)HttpStatusCode.Conflict);
            }

            return Ok(GetBasketModel(basket.Single()));
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            return Ok(GetBasketModel(basket));
        }

        private BasketModel GetBasketModel(Basket basket) {
            var basketModel = basket.ToBasketModel();
            var id = basket.Id;
            basketModel.AddLink(("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}")));
            basketModel.AddLink(("contents", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items")));
            basketModel.AddLink(("catalogue", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue")));


            if (basket.Status == BasketStatus.Open) {
                basketModel.AddLink(("checkout", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/checkout")));
            }
            return basketModel;
        }

        [HttpDelete("{id}/items")]
        public IActionResult ClearContents(string id) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }
            _basketRepository.ClearItems(id, _clock.Now);
            _basketRepository.UpdateBasketTotal(id, 0, _clock.Now);
            return Ok();
        }

        [HttpGet("{id}/items")]
        public IActionResult GetContents(string id) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            var basketItems = new List<BasketItemModel>();

            foreach (var item in basket.Items) {
                var itemModel = item.ToBasketItemModel();
                itemModel.AddLinks(("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items/{item.CatalogueItemId}")));
                itemModel.AddLinks(("details", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue/{item.CatalogueItemId}")));
                basketItems.Add(itemModel);
            }

            var links = new Link[] {
                ("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items")),
                ("basket", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}"))
            };
            return Ok(new CollectionResult<BasketItemModel>(basketItems, links));
        }

        [HttpGet("{id}/catalogue")]
        public IActionResult GetCatalogue(string id, [FromQuery]int page) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            var pageSize = 20;
            var catalogueItems = _catalogue.GetAll().Skip(page * pageSize).Take(pageSize);

            var stockItems = new List<StockItemModel>();
            foreach (var item in catalogueItems) {
                var itemModel = item.ToStockItemModel();
                itemModel.AddLinks(("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue/{item.Id}")));
                if (!basket.Items.Any(b => b.CatalogueItemId == item.Id)) {
                    itemModel.AddLinks(("add", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items/{item.Id}")));
                }
                stockItems.Add(itemModel);
            }


            int totalItems = _catalogue.GetCount();
            int totalPages = (int)Math.Ceiling(totalItems / 20d);
            int pageNumber = page + 1;

            var pagedLinks = new List<Link> {
                ("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue?page={page}")),
                ("basket", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}"))
            };
            if (totalPages != 0) {
                pagedLinks.Add(("first", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue")));
                pagedLinks.Add(("last", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue?page={totalPages - 1}")));


                if (pageNumber < totalPages) {
                    pagedLinks.Add(("next", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue?page={pageNumber}")));
                }

                if (pageNumber > 1) {
                    pagedLinks.Add(("prev", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue?page={page - 1}")));
                }
            }

            return Ok(new PaginatedCollectionResult<StockItemModel>(stockItems, pageNumber, totalPages, totalItems, pagedLinks.ToArray()));
        }

        [HttpGet("{id}/items/{catalogueItemId}")]
        public IActionResult GetBasketItem(string id, string catalogueItemId) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            var item = basket.Items.SingleOrDefault(b => b.CatalogueItemId == catalogueItemId)?.ToBasketItemModel();
            if (item == null) {
                return NotFound();
            }

            item.AddLinks(("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items/{catalogueItemId}")));
            item.AddLinks(("details", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue/{catalogueItemId}")));
            item.AddLinks(("basket", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}")));
            item.AddLinks(("basket.contents", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items")));

            return Ok(item);
        }

        [HttpPut("{id}/items/{catalogueItemId}")]
        public IActionResult AddOrUpdateItem(string id, string catalogueItemId, [FromBody] BasketItemRequest basketItemRequest) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            var item = _catalogue.Get(catalogueItemId);
            if (item == null) {
                return NotFound();
            }

            var existingItem = basket.Items.FirstOrDefault(b => b.CatalogueItemId == catalogueItemId);
            if (existingItem != null) {
                _basketRepository.UpdateItem(
                    id,
                    catalogueItemId,
                    basketItemRequest.Quantity,
                    basketItemRequest.Quantity * existingItem.Price,
                    _clock.Now);
            }
            else {

                _basketRepository.AddItem(
                    id,
                    new BasketItem(
                        catalogueItemId,
                        item.Name,
                        item.Description,
                        item.Price,
                        basketItemRequest.Quantity,
                        item.Price * basketItemRequest.Quantity),
                    _clock.Now);
            }

            var newBasket = _basketRepository.Get(id);
            var newTotal = newBasket.Items.Sum(it => it.Total);

            _basketRepository.UpdateBasketTotal(id, newTotal, _clock.Now);

            return Ok();
        }

        [HttpGet("{id}/catalogue/{catalogueItemId}")]
        public IActionResult GetCatalogueItem(string id, string catalogueItemId) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            var item = _catalogue.Get(catalogueItemId)?.ToStockItemModel();
            if (item == null) {
                return NotFound();
            }

            item.AddLinks(("self", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue/{catalogueItemId}")));
            if (!basket.Items.Any(b => b.CatalogueItemId == catalogueItemId)) {
                item.AddLinks(("add", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/items/{catalogueItemId}")));
            }
            item.AddLinks(("basket", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}")));
            item.AddLinks(("catalogue", new Uri(MyHttpContext.AppBaseUrl, $"api/baskets/{id}/catalogue")));

            return Ok(item);
        }


        [HttpDelete("{id}/items/{catalogueItemId}")]
        public IActionResult RemoveItem(string id, string catalogueItemId) {
            var basket = _basketRepository.Get(id);
            if (basket == null) {
                return NotFound();
            }

            try {
                _basketRepository.RemoveItem(id, catalogueItemId, _clock.Now);
            }
            catch (ArgumentException) {
                return NotFound();
            }

            var newBasket = _basketRepository.Get(id);
            var newTotal = newBasket.Items.Sum(it => it.Total);

            _basketRepository.UpdateBasketTotal(id, newTotal, _clock.Now);
            return Ok();
        }
    }
}
