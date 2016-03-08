# Scope

I chose to focus heavily on the Backend:
* RESTful Auction Service: auction info; place a bid
* WebSocket-based Notification Service to enable interactivity: Auction Updates
* Both RESTful and WebSocket API are consumed by a single-page HTML5 App
  (AuctionApp.html and Utils.js)
* jQuery to consume RESTful API
* HTML5 Web Sockets to consume the WebSockets-based Notification Service

# Architecture

First step is to build the Auction Service (Business Logic) which would serve our requests, such as GetAuction or PlaceBid.

We've got two problems here:
* where to store data
* how to make sure that our service is concurrent (several users placing a bid at the same time)

## As to ensuring the concurrency there are two options:
* Make our servie a singleton and wrap the PlaceBid functionality into lock
  * Pros: simple
  * Cons: not scalable when we have multiple services running, incl. behing an NLB
* Implement a singleton Synchoronization Service
  * Pros: in the manner of miroservices, flexiblity
  * Cons: overkill, too complicated
* Let maybe the data store handle concurrency, since usually data stores are designed with concurrency in mind to guarantee data integrity. After all all we need to make sure that we update data in an atomic fashion when placing a bid.

## Options for Storing Data 
* SQL DB
  * Cons: Too complicated to set up,  difficult ot maintain, overkill, transactions/locks are just ugly
* In-Memory (eg. using Lists of Dictionaries)
  * Pros: simple, concurrency achieved by using locks
  * Cons: not scalable, data not persisted
* REDIS
  * Pros: simple to set up, replication & clustering supported, single-threaded => inherently concurrent, persistable

So in the end I opted for REDIS as it was a very simple and highly scalable solution.
## REDIS

REDIS comes handy here in two major ways:
* To get auction Id I simply use INCR to atomically increase the value of a key NextAuctionId
* As to the PlaceBid functionality I implemented it via a LUA Script which is executed atomically on REDIS:

```lua
local tryPlaceBid = function (auctionKey, bidder, bid)

	local prices = redis.call('hmget', auctionKey,'HighestBid', 'StartingPrice')		
	local highestBid = prices[1];	
	local startingPrice =  prices[2];
	
	local bidNumber = tonumber(bid)

	if highestBid == false and startingPrice == false then
		return false
	end
	
	-- NB: check if prices exist (non false), otherwise we run int a error: cannot compare nil to int
	if(highestBid ~= false and tonumber(highestBid) >= bidNumber) or (startingPrice ~= false and tonumber(startingPrice) >= bidNumber)
	then
		return false
	else 
		redis.call('hmset',auctionKey,'HighestBid',bid, 'HighestBidder',bidder)
		redis.call('hincrby', auctionKey, 'Updated', 1)
		return redis.call('hgetall', auctionKey)
	end
end

return(tryPlaceBid(KEYS[1],ARGV[1],ARGV[2]))
```
## API
Auction Service API should define API to handle the following tasks:
* GetAuction
* GetAuctions (incl. pagination)
* Place a bid

but also the following tasks should be atomated:
* Open an auction
* Delete an auction

As the latter two are quite dangerous, the Auction Service should expose actually two RESTful Endpoints:
* BuyerService/... which defines operations for a buyer
* AuctionService/... which defines all of the above + Open & Delete an auction

## Notification Service
Sure our RESTful API addresses the task, but it doesn enable interactivity.
So we need to build a separate Notification Service (akin to microservices design):
* When placing a bid, an Auction Service would publish an update to the Notification Service
* Notifation Service informs all interested parties via WebSockets
* Notification Service should be a singleton (as it maintains internally open WS-Sessions), but it is possible to have multiple Notification Services grouped per AuctionIDs.

# Choosing technologies

*
