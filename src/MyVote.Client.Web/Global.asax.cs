using System;
using System.Web.Optimization;

namespace MyVote.Client.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/app")
                .Include(
                    "~/Scripts/MobileServices.Web-{version}.js",
                    "~/Scripts/postmessage.js",
                    "~/Scripts/moment.js",
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/angular.ieCors.js",
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-route.js",
                    "~/Scripts/angular-resource.js",
                    "~/Scripts/highcharts/highcharts.js",
                    "~/Scripts/angular-highcharts.js",
                    "~/Scripts/jquery.signalR-{version}.js",
                    "~/app/app.js",
                    "~/app/models/*.js",
                    "~/app/services/*.js",
                    "~/app/controllers/*.js",
                    "~/app/directives/*.js"));

            BundleTable.Bundles.Add(new StyleBundle("~/bundles/css")
                .Include(
                    "~/Content/site.css",
                    "~/Content/landing.css",
                    "~/Content/poll.css"));
        }
    }
}