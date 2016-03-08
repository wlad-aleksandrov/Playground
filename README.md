# Scope

I chose to focus heavily on the Backend:
* RESTful Auction Service: auction info; place a bid
* WebSocket-based Notification Service to enable interactivity: Auction Updates
* Both RESTful and WebSocket API are consumed by a single-page HTML5 App
  (AuctionApp.html and Utils.js)
** jQuery to consume RESTful API
** HTML5 Web Sockets to consume the WebSockets-based Notification Service

# Architecture

First step is to build the Auction Service (Business Logic) which would serve our requests, such as GetAuction or PlaceBid.

We've got two problems here:
* where to store data
* how to make sure that our service is concurrent (several users placing a bid at the same time)

## As to ensuring the concurrency there are two options:
* Make our servie a singleton and wrap the PlaceBid functionality into lock
  ** Pros: simple
  ** Cons: not scalable when we have multiple services running, incl. behing an NLB
* Implement a singleton Synchoronization Service
  ** Pros: in the manner of miroservices, flexiblity
  ** Cons: overkill, too complicated
* Let maybe the data store handle concurrency, since usually data stores are designed with concurrency in mind to guarantee data integrity. After all all we need to make sure that we update data in an atomic fashion when placing a bid.

## Options for Storing Data 
* SQL DB
  ** Cons: Too complicated to set up, transactions/locks are just ugly
* In-Memory (eg. using Lists of Dictionaries)
  ** Pros: simple, concurrency achieved by using locks
  ** Cons: not scalable, data not persisted
* REDIS
  ** Pros: simple to set up, replication & clustering supported, single-threaded => concurrent, persistable

So in the end I opted for REDIS as it was a very simple and highly scalable solution.



# Choosing technologies

*
