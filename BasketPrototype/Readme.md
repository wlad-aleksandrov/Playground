
# Introduction

This is a prototype of a Basket API. It is implemented as a hypermedia-driven (HATEOAS) RESTful Service using ASP.NET CORE 2.

Solution BasketPrototype.sln is in the folder BasketPrototype.

There are five projects:

1. BasketRepository - Data Storage for Buskets
2. Inventory - Data Storage for Stock
3. BasketService - RESTful API
4. BasketApiClient - a low-level client lib
5. BasketApiClientTestApp - a sample app using the client lib

# HATEOAS

To enable HATEOAS I initially chose halcyon (https://github.com/visualeyes/halcyon) as a HAL-implementation for ASP.NET CORE
(http://stateless.co/hal_specification.html and https://tools.ietf.org/html/draft-kelly-json-hal-06).

However due to some missing functionality related to templated for collection items, 
I eventually abandoned the HAL-format (application/json+hal) and instead extended existing DTOs with a new attribute _Links.

Attribute _expanded (expanding subresources) is not yet supported by this API.


# WORKFLOW

There is just one gateway to fire requests: http://localhost:5050/api/baskets.
A client is guided exclusively by links. 
Dependending on the current station of the workflow a client is presented with different links to navigate.

1. A client creates a basket by POSTing under the gateway. 
   She then reseives a location and no body (might be nice to return a created basket as well here though).

2. A client GETs location to get a representation of the basket.
The basket has four links:
    * catalogue: to browse catalogue and add items from the catalogue to the basket
    * contents: to browse items in the basket, update quantity, delete individual or all items
    * self: returned always by default
    * checkout: this link is only returned if the Basket Status is OPEN.     
    Should the basket has already been paid, the response would not contain the checkout link.

3. A client browses catalogue by GETting link to catalogue.
Link returns a page of items with a bunch of navigational links to navigate between pages
(next, prev, last, first).
Each catalogue item has also a self link and additionally a link to PUT this item straight to the basket.
A link to PUT item to the basket (add) is only available, if an item is not already in the basket.

4. A client browses basket items by GETting link to contents.
Every basket collection has the following links:
    * self: to update quantity or delete an item
    * details: to guide the client to catalogue to learn more about this item
    (Basket item contains just a subset of attributes of a catalogue item.)

# OPEN POINTS
    1. Tests
    2. Model Validation in Basket Service
    3. HTTPs and CORS
