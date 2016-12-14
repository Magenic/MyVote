using System;
using Microsoft.AspNetCore.SignalR;

namespace MyVote.UI.Web.NetCore
{
    /// <summary>
    /// SignalR Hub Class
    /// </summary>
    public class MyVoteHub : Hub
    {

        //Method name matching is case-insensitive. For example, Clients.Others.pollAdded on the server will execute PollAdded or pollAdded on the client.
        //[Authorize(Roles = "Admin")]
        public void AddPoll()
        {
            var dateTimeString = DateTime.Now.ToString("O");
            //Call the pollAdded listener on the client
            //Send message to all connected clients except sender 
            Clients.Others.pollAdded(dateTimeString);
            //Clients.All.pollAdded(dateTimeString);  <--Thanks Captain Obvious
        }

    }
}
