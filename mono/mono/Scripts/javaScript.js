(function (signalRFunctions, $, undefined) {
    var messagesDiv = '<div id="messages" style="width:300px;position:fixed;top:80px;right:30px;"></div>';

    //Private Method
    function writeError(line) {
        write(line, "alert-danger")
    }

    //Private Method
    function writeNotification(line) {
        write(line, "alert-info")
    }

    //Private Method
    function writeMessage(line) {
        write(line, "alert-success")
    }

    //Private Method
    function getTimeString() {
        var currentTime = new Date();
        return currentTime.toLocaleTimeString();
    }

    //Private Method
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

        $.ajax({
            url: "/Auction/Order/offersCount"
        }).done(function (data) {
            $("#offersCount").html("offers(" + data + ")");
        });

        $("body").append(messagesDiv);

        var connection = $.connection.hub;
        var hub = $.connection.chatHub;

        connection.logging = true;

        hub.client.message = function (value) {
            writeMessage(value);
        }

        connection.start()
            .done(function () {
                writeNotification("connected");
            })
            .fail(function (error) {
                writeError(value);
            });

        $("#sendToRestaurants").click(function () {
            hub.server.sendToGroupRestaurant($("#message").val());
        });
    }

    //Public Method
    signalRFunctions.restaurantClient = function () {
        $.ajax({
            url: "/Auction/Offer/ordersCount"
        }).done(function (data) {
            $("#ordersCount").html(data);
        });

        $("body").append(messagesDiv);

        var connection = $.connection.hub;
        var hub = $.connection.chatHub;

        connection.logging = false;

        hub.client.hubMessage = function (data) {
            writeMessage(data);
        }

        hub.client.hubNotification = function (data) {
            writeNotification(data);
        }

        connection.start()
            .done(function () {
                writeNotification("connected");
                hub.server.joinGroupRestaurant();
            })
            .fail(function (error) {
                writeError(value);
            });

        $("#sendToUser").click(function () {
            hub.server.sendToUser($("#userId").val(), $("#message").val());
        });
    }

}(window.signalRFunctions = window.signalRFunctions || {}, jQuery));