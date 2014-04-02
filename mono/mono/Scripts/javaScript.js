(function (signalRFunctions, $, undefined) {
    //Private Property
    var messagesDiv = '<div id="messages" style="width:300px;position:fixed;top:80px;right:30px;"></div>';

    //Private Method
    function writeError(line) {
        write(line, "alert-danger")
    }

    function writeNotification(line) {
        write(line, "alert-info")
    }

    function writeMessage(line) {
        write(line, "alert-success")
    }

    function getTimeString() {
        var currentTime = new Date();
        return currentTime.toLocaleTimeString();
    }

    function write(line, type) {
        var messages = $("#messages");

        var mes = $('<div class="alert ' + type + '"><button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>' + getTimeString() + ': ' + line + '</div>');
        messages.append(mes);

        var delayTime = 3000;
        var fadeoutTime = 400;

        mes.delay(delayTime).fadeOut(fadeoutTime);

        setTimeout(function () {
            mes.remove();
        }, delayTime + fadeoutTime);
    }

    //Public Method
    signalRFunctions.userClient = function () {
        $.ajax({
            url: "/Auction/Basket/ItemsInBasket"
        }).done(function (data) {
            $("#itemsInBasket").html(data);
        });

        function updateOffersCount() {
            $.ajax({
                url: "/Auction/Order/OffersCount"
            }).done(function (data) {
                $("#offersCount").html("offers(" + data + ")");
            });
        }

        updateOffersCount();

        $("body").append(messagesDiv);

        var connection = $.connection.hub;
        var hub = $.connection.chatHub;

        //connection.logging = true;

        function offersCountForOrder(orderId) {
            updateOffersCount();

            var order = $("#offersCountForOrder" + orderId);

            if (order.length > 0) {
                $.ajax({
                    url: "/Auction/Order/OffersCountForOrder/" + orderId
                }).done(function (data) {
                    order.html(data);
                });
            }
        }

        hub.client.offersCountForOrderNew = function (orderId, offerID) {
            writeMessage("New offer");
            offersCountForOrder(orderId);

            var offers = $("#offers");

            if (offers.length > 0) {
                $.ajax({
                    url: "/Auction/Order/offer/" + offerID
                }).done(function (data) {
                    offers.prepend(data);
                });
            }
        }

        hub.client.offersCountForOrderDeleted = function (orderId, offerID, message) {
            writeError("Offer deleted");
            offersCountForOrder(orderId);

            var offer = $("#offer" + offerID);

            if (offer.length > 0) {
                offer.remove();
            }
        }

        connection.start()
            .done(function () {
                //writeNotification("connected");
            })
            .fail(function (error) {
                writeError(value);
            });
    }

    signalRFunctions.restaurantClient = function () {

        function updateOrdersCount() {
            $.ajax({
                url: "/Auction/Offer/ordersCount"
            }).done(function (data) {
                $("#ordersCount").html(data);
            });
        }

        updateOrdersCount();

        $("body").append(messagesDiv);

        var connection = $.connection.hub;
        var hub = $.connection.chatHub;

        //connection.logging = true;

        hub.client.removeOrder = function (orderId) {
            updateOrdersCount();
            writeError("Order removed");

            var order =  $("#order" + orderId);

            if (order.length > 0) {
                order.remove();
            }
        }

        hub.client.addOrder = function (orderId) {
            updateOrdersCount();
            writeMessage("New order")

            var orders = $("#orders");

            if (orders.length > 0)
            {
                $.ajax({
                    url: "/Auction/Offer/order/" + orderId
                }).done(function (data) {
                    orders.prepend(data);
                });
            }
        }

        connection.start()
            .done(function () {
                //writeNotification("connected");
                hub.server.joinGroupRestaurant();
            })
            .fail(function (error) {
                writeError(value);
            });
    }

}(window.signalRFunctions = window.signalRFunctions || {}, jQuery));

(function (imageFunctions, $, undefined) {
    imageFunctions.changeImageEvent = function () {
        $("#PhotoID").change(function () {
            var fileName = $("#PhotoID option:selected").text();
            var sImage = $("#selectedImage");

            if (fileName === "")
                sImage.attr("src", "/Content/Images/_.png");
            else
                sImage.attr("src", "/Content/Images/" + fileName + ".png");
        });
    }

}(window.imageFunctions = window.imageFunctions || {}, jQuery));