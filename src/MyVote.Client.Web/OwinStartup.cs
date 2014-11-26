using Microsoft.Owin;
using Owin;

//Register this class so when OWIN starts up, it calls Configuration method and maps routes to SignalR Hub
[assembly: OwinStartup(typeof(MyVote.Client.Web.OwinStartup))]

namespace MyVote.Client.Web
{
    /// <summary>
    /// Bootstrap to map SignalR routes to Hub
    /// </summary>
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
