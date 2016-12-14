using Autofac;
using Autofac.Extensions.DependencyInjection;
using Csla;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyVote.BusinessObjects;
using MyVote.Data.Entities;
using MyVote.Services.AppServer.Auth;
using MyVote.Services.AppServer.Filters;
using System;

namespace MyVote.Services.AppServer
{
	public partial class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				 .SetBasePath(env.ContentRootPath)
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				 .AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            //Configure CORS middleware before MVC
            services.AddCors(_ => _.AddPolicy(Constants.CorsPolicyName, b =>
            {
                b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            services.AddMvc()
              .AddJsonOptions(options =>
              {
                  options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                  options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
              });

            var builder = services.AddMvc();
			builder.AddMvcOptions(_ => { _.Filters.Add(new UnhandledExceptionFilter()); });

			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule(new BusinessObjectsModule());
			containerBuilder.RegisterModule(new EntitiesModule());
			containerBuilder.RegisterModule(new AuthModule());
			containerBuilder.Populate(services);

			var container = containerBuilder.Build();

			ApplicationContext.DataPortalActivator = new ObjectActivator(
				container, new ActivatorCallContext());

			return container.Resolve<IServiceProvider>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

            ConfigureAuth(app);

            //Add CORS middleware before MVC
            app.UseCors(Constants.CorsPolicyName);

            app.UseMvc();
		}
	}
}
