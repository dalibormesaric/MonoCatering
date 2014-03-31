using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace Mono
{
    public class ChatHub : Hub
    {
        [Authorize(Roles = "user")]
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        [Authorize(Roles = "user")]
        public void SendToGroupRestaurant(string offerID)
        {
            Clients.Group("restaurant").hubMessage(offerID);
        }

        [Authorize(Roles = "restaurant")]
        public void SendToUser(string userId, string orderID)
        {
            Clients.User(userId).message(orderID);
        }

        [Authorize(Roles = "restaurant")]
        public void JoinGroupRestaurant()
        {
            Groups.Add(Context.ConnectionId, "restaurant");
            //Clients.All.hubMessage(Context.ConnectionId + " joined group restaurant");
            Clients.All.hubNotification("joined group restaurant");
        }

    }
}