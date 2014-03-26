(function (signalRFunctions, $, undefined) {
    //Private Method
    function writeError(line) {
        var messages = $("#messages");
        messages.append("<li style='color:red;'>" + getTimeString() + ' ' + line + "</li>");
    }

    //Private Method
    function writeLine(line) {
        var messages = $("#messages");
        messages.append("<li style='color:black;'>" + getTimeString() + ' ' + line + "</li>");
    }

    //Private Method
    function getTimeString() {
        var currentTime = new Date();
        return currentTime.toTimeString();
    }

    //Public Method
    signalRFunctions.userClient = function() {
        var connection = $.connection.hub,
            hub = $.connection.chatHub;

        connection.logging = true;

        hub.client.message = function (value) {
            writeLine(value);
        }

        connection.start()
            .done(function () {
                writeLine("connected");
            })
            .fail(function (error) {
                writeError(value);
            });

        $("#sendToRestaurants").click(function () {
            hub.server.sendToGroupRestaurant($("#message").val());
        });
    }

    //Public Method
    signalRFunctions.restaurantClient = function() {
        var connection = $.connection.hub,
        hub = $.connection.chatHub;

        connection.logging = true;

        hub.client.hubMessage = function (data) {
            writeLine("hubMessage: " + data);
        }

        connection.start()
            .done(function () {
                writeLine("connected");
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