using BasketApiClient;
using System;
using System.Linq;
using BasketApiClient.DTO;

namespace BasketApiClientTestApp {
    class Program {
        static void Main(string[] args) {
            var basketService = new BasketEndpointApi();

            var location = basketService.CreateBasket(new Uri("http://localhost:5050/api/baskets"), $"Vlad+{Guid.NewGuid()}").Result;
            var basket = basketService.GetBasket(location).Result;

            var contentsUri = basket._Links.SingleOrDefault(l => l.Rel == "contents").Href.ToUri();
            var basketItems = basketService.GetContents(contentsUri).Result;

            var catalogueUri = basket._Links.SingleOrDefault(l => l.Rel == "catalogue").Href.ToUri();

            var stockItems = basketService.GetCatalogueItems(catalogueUri).Result;
            var stockItemsLastPage = basketService.GetCatalogueItems(stockItems._Links.SingleOrDefault(l => l.Rel == "last").Href.ToUri()).Result;

            var item1Add = stockItemsLastPage.Items[0]._Links.SingleOrDefault(l => l.Rel == "add").Href.ToUri();
            var item2Add = stockItemsLastPage.Items[1]._Links.SingleOrDefault(l => l.Rel == "add").Href.ToUri();
            var item3Add = stockItemsLastPage.Items[2]._Links.SingleOrDefault(l => l.Rel == "add").Href.ToUri();

            basketService.AddOrUpdateBasketItem(item1Add, 12).GetAwaiter().GetResult();
            basket = basketService.GetBasket(location).Result;

            basketService.AddOrUpdateBasketItem(item1Add, 4).GetAwaiter().GetResult();
            basket = basketService.GetBasket(location).Result;

            basketService.AddOrUpdateBasketItem(item3Add, 12).GetAwaiter().GetResult();
            basket = basketService.GetBasket(location).Result;

            basketService.AddOrUpdateBasketItem(item2Add, 1).GetAwaiter().GetResult();
            basket = basketService.GetBasket(location).Result;

            basketService.Remove(item2Add).GetAwaiter().GetResult();
            basket = basketService.GetBasket(location).Result;
            basketService.Remove(contentsUri).GetAwaiter().GetResult();
            basket = basketService.GetBasket(location).Result;
        }
    }
}
