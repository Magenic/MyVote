using System;
using Microsoft.AspNet.SignalR;

namespace MyVote.Client.Web
{
    public class MyVoteHub : Hub
    {
        public void AddPoll()
        {
            var dateTimeString = DateTime.Now.ToString("O");
            Clients.Others.pollAdded(dateTimeString);
        }
    }
}