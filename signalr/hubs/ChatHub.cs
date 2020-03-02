using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace FYP.signalr.hubs
{
    public class ChatHub : Hub
    {
        public void Send(string user, string messge)
        {
            Clients.All.addNewMessageToPage(user, messge);
        }
    }
}