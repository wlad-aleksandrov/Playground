'use strict';

/*angular.module('auctionApp', [])
  .controller('AuctionsCtrl', function ($scope, $http) {
      $http.jsonp('http://localhost:8733/BuyerService/Auctions/12?callback=angular.callbacks._0').then(function (auctionsResponse) {
          $scope.auctions = auctionsResponse.data;
      });
  })*/

angular.module('auctionApp', [])
controller('AuctionsCtrl', function ($scope, $http) {
    $http.jsonp('http://localhost:8733/BuyerService/Auctions/12?callback=angular.callbacks._0').then(function (auctionsResponse) {
        $scope.auctions = auctionsResponse.data;
    });
})
  .factory('auction', function () {
      var auction = null;
      return {
          getAuction: function (id) {
              $http.jsonp('http://localhost:8733/BuyerService/Auctions/12?callback=angular.callbacks._0').then(function (auctionsResponse)
              {
                  auction = auctionsResponse.data;
              });
          },
          addArticle: function (article) {
              items.push(article);
          },
          sum: function () {
              return items.reduce(function (total, article) {
                  return total + article.price;
              }, 0);
          }
      };
  })