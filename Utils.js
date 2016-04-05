function getAuction() {
    $.ajax({
        url: 'http://localhost:9090/AuctionService/Auctions/' + $('#auctionId').val(),
        success: function (auction)
        {
            $('#tableAuctionId').text(auction.Id);
            $('#tableName').text(auction.Name);
            $('#tableDescription').text(auction.Description);
            $('#tableImageUrl').text(auction.ImageUrl);
            $('#tableStartingPrice').text(auction.StartingPrice);
            $('#tableHighestBidder').text(auction.HighestBidder);
            $('#tableHighestBid').text(auction.HighestBid);

            var ws = new WebSocket("ws://localhost:8089/AccountService");

            ws.onopen = function () {
                ws.send("Subscribe " + $('#auctionId').val());
            };

            ws.onmessage = function (evt) {

                var data = JSON.parse(evt.data);
                // check if supplied value is greater than existing one
                if(parseInt($('#tableHighestBid').text()) < data.HighestBid)
                {     
                     $('#tableHighestBidder').text(data.Bidder);
                     $('#tableHighestBid').text(data.HighestBid);
                }
            };
        }
    });
}

function placeBid() {
    var payload = JSON.stringify( { bidderId: $('#bidderId').val(), bidAmount: $('#bidAmount').val() });

    $.ajax({
        url: 'http://localhost:9090/AuctionService/Auctions/' + $('#auctionId').val(),
        type: 'PUT',
       // crossDomain: true,
        contentType: "application/json",
        data: payload,
        processData:false,
        success: function (data) {

            if (data)
                $('#lastStatus').text("Last Status: SUCCESS");
            
            else
                $('#lastStatus').text("Last Status: DECLINED");
        },
        error: function (er)
        { }
    });
}