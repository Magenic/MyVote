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
                    "~/Scripts/jquery-ui-{version}.js",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/angular.ieCors.js",
                    "~/Scripts/angular.js",
                    "~/Scripts/angular-route.js",
                    "~/Scripts/angular-resource.js",
                    "~/Scripts/highcharts/highcharts.js",
                    "~/Scripts/angular-highcharts.js",
                    "~/Scripts/angular-ui/ui-bootstrap.js",
                    "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                    "~/Scripts/jquery.signalR-{version}.js",
                    "~/Scripts/modernizr-{version}.js",
                    "~/Scripts/polyfiller.js",                    
                    "~/app/app.js",
                    "~/app/models/*.js",
                    "~/app/services/*.js",
                    "~/app/controllers/*.js",
                    "~/app/directives/*.js"));

            BundleTable.Bundles.Add(new StyleBundle("~/bundles/css")
                .Include(
                    "~/Content/jquery-ui.css",
                    "~/Content/bootstrap.css",
                    "~/Content/site.css",
                    "~/Content/landing.css",
                    "~/Content/poll.css"
                    ));


            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            #if (RELEASE)
            {  
                BundleTable.EnableOptimizations = true;
            }
            #else
            {
                BundleTable.EnableOptimizations = false;
            }
            #endif 

        }
    }
}