using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MyVote.Client.Web.OwinStartup))]

namespace MyVote.Client.Web
{
    public class OwinStartup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
