# Scope

I chose to focus heavily on the Backend:
* RESTful Auction Service: auction info; place a bid
* WebSocket-based Notification Service to enable interactivity: Auction Updates
* Both RESTful and WebSocket API are consumed by a single-page HTML5 App
  (AuctionApp.html and Utils.js)
  * jQuery to consume RESTful API
  * HTML5 Web Sockets to consume the WebSockets-based Notification Service

# Architecture

![Architecture Diagram](/Auction App Diagram.png?raw=true "App Diagram")

First step is to build the Auction Service (Business Logic) which would serve our requests, such as GetAuction or PlaceBid.

We've got two problems here:
* where to store data
* how to make sure that our service is concurrent (several users placing a bid at the same time)

## As to ensuring the concurrency there are two options:
* Make our service a singleton and wrap the PlaceBid functionality into lock
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
* Basically PUB/SUB

* REDIS also supports PUB/SUB pattern (though somewhat lowlevel), but the Notification Service should not depend on the Redis Repository. Besides it should be the responsibility of the Auction Service as business logic part to publish updates.

# Summarizing Architecture

1. We need an Auction Repository DLL as a wrapper around REDIS (or other future resository solutionS)
2. We need an AuctionService DLL to provide RESTful API
 * It will consume the RepositoryDLL for some of buisness logic and persistence purposes
 * It will implement the rest of business logic
 * It will consume some NotificationClient CLL to publish updates to the NotificationService
3. As for NotificationDLL we probably need to enforce thread-safety/concurrency
 * It will be a web sockets client
 * lock or Producer/Consumer Pattern would do ==> Producer/Consumer Pattern based on BlockingCollection
4. NotificationService, should be very simple: Publsih & Subscribe
5. As for Auction Images --> we will store just image urls. Clients would then download images from a dedicated web server, which is beyond the scope this project.

# Technologies

1. For RESTful AucountService I chose WCF, which I know very well. It really fits the bill, although on MSDN it says that ASP.NET 5 WebAPi should be the preferred option to develop REST services.
 * Alternatives: ServiceStack (which is paid unless you use an old version) and ASP.NET 5 WebAPI.
 * ASP.NET 5 WebAPI: DNX required, not sure how to host as a Windows Service, or as a normal process without IIS overhead.

2. For REDIS I chose StackExchange.Redis (open-source and maintained). 
  Although Redis is built primarily for Linux, there is a port to Windows by MS Open Tech @ https://github.com/ServiceStack/redis-windows. Besides there is a cloud option with Azure Redis.

3. For WebSockets I chose SuperSocket (SuperSocketWebSocket), which is an open-source, and very mature framework. 
 * Supports TLS/SSL. 
 * What I like about SuperSocket is that we can consume API via browser HTML5 WebSockets API. No third-party js libs required.
 * Works on iOS and Android (Apache Cordova HTML5 Apps).

# DI & IoC

Both services make use of Dependency Injection, but no IoC are used (not really needed). 
Dependencies are injected manually via hosts with or without a factory.
 * Repository for Auction Service : default REDIS, but possible to design outher solutions to be used as defined in a config file (SQL, memcached, in-memory, hybrid (SQL for Persistence /No-SQL for the rest) etc...)
 * Notification Client for Auction Service: currently concurrent PRODUCER/CONSUMER WebSockets, but possible to replace with other solutions: different concurrent implementations, multi-threaded...
 * Notification Server for Notification Service: via factory
 

# Tests

1. Manual Tests & Concurrency Tests via JMeter (Uploaded TestRedis.jmx) to test RESTful API
2. Manual Tests for WebSockets & REST via a HTML5 App
3. Unit Tests probably should have been implemented... although the code is really simple when it comes to the cyclomatic complexity.
4. JMeter Tests can be enhanced to enable automated integration tests.

# What remains to be done
1. Critical: TLS/SSL: should be however straightforward: settings added to config files + tests
2. Not critical: NotificationSerivce going offline... currently reconnect implemented, but should the Serivce be down for good, we should come up with a better solution.
3. Authentication for Buyers via REDIS + separate set of Buyers with name, country etc. (currently generic BidderID is used)
4. Hosts: currently both Services are Console Apps. Options:
* as a Windows Service or IIS Service (esp. for WCF)
* Background Process (Console Apps without Console Output) and a Synchorization/Management Service to control (start/stop...) the background processes.
