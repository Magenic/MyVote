using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Owin;

[assembly: OwinStartup(typeof(MyVote.Services.MobileApp.Startup))]

namespace MyVote.Services.MobileApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}