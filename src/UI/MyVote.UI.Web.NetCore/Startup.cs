using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MyVote.UI.Web.NetCore
{
    /// <summary>
    /// The Startup class configures the request pipeline that handles all requests made to the application
    /// </summary>
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // Adding services to the services container makes them available within your application via dependency injection.
        // The ConfigureServices method is called before Configure
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.AddSignalR(options => options.Hubs.EnableDetailedErrors = true);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // The Configure method is used to specify how the ASP.NET application will respond to HTTP requests
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            env.EnvironmentName = EnvironmentName.Development;
            if (env.IsDevelopment())
            {
                //Configure app to display a page that shows detailed information about exceptions
                app.UseDeveloperExceptionPage();
            }

            //Each 'Use' extension method adds a middleware component to the request pipeline. 
            //the 'UseMvc' extension method adds the routing middleware to the request pipeline and configures MVC as the default handler

            app.UseCors(builder => builder.AllowAnyOrigin());

            //Configure to serve a default page
            app.UseDefaultFiles();
            //Configure for static files to be served outside of wwwroot
            app.UseStaticFiles();

            //Add our MyVoteHub via UseSignalR extension method
            app.UseSignalR(routes =>
            {
                routes.MapHub<MyVoteHub>("myvotehub");
            });
        }
    }
}
