using System;
using Microsoft.AspNet.SignalR;

namespace MyVote.Client.Web
{
    /// <summary>
    /// SignalR Hub class
    /// </summary>
    public class MyVoteHub : Hub
    {
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