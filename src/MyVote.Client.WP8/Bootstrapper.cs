using Autofac;
using Caliburn.Micro;
using MyVote.BusinessObjects;
using MyVote.UI;
using MyVote.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.Client.WP8
{
	public sealed class Bootstrapper : PhoneBootstrapper
	{
		private IContainer Container { get; set; }

		protected override void Configure()
		{
			Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
#if DEBUG
			Csla.ApplicationContext.DataPortalUrlString = "http://localhost:15440/MobilePortal.svc";
#else

#if STAGING
			Csla.ApplicationContext.DataPortalUrlString = "http://084dd66925e34ddca11a8277ceb0ded4.cloudapp.net/MobilePortal.svc";
#else
			Csla.ApplicationContext.DataPortalUrlString = "http://myvote.cloudapp.net/MobilePortal.svc";
#endif // STAGING

#endif // DEBUG

			var containerBuilder = new ContainerBuilder();

			containerBuilder.RegisterInstance<INavigationService>(new FrameAdapter(this.RootFrame)).SingleInstance();
			new UiContainerBuilderComposition().Compose(containerBuilder);
			new BusinessObjectsContainerBuilderComposition().Compose(containerBuilder);

			this.Container = containerBuilder.Build();

			// Due to our cross platform goals, our namespaces don't match our assembly names.
			// Caliburn.Micro assumes they would be the same, so we have to override this
			// functionality to account for it.
			ViewLocator.DeterminePackUriFromType = (viewModelType, viewType) =>
			{
				var assemblyName = viewType.GetTypeInfo().Assembly.GetAssemblyName();

				var uri = viewType.FullName.Replace(assemblyName.Replace(".WP8", string.Empty), string.Empty).Replace(".", "/") + ".xaml";

				if (!System.Windows.Application.Current.GetType().GetTypeInfo().Assembly.GetAssemblyName().Equals(assemblyName))
				{
					return "/" + assemblyName + ";component" + uri;
				}

				return uri;
			};
		}

		protected override object GetInstance(Type service, string key)
		{
			object instance = null;

			if (string.IsNullOrWhiteSpace(key))
			{
				if (this.Container.TryResolve(service, out instance))
				{
					return instance;
				}
			}
			else
			{
				if (this.Container.TryResolveNamed(key, service, out instance))
				{
					return instance;
				}
			}

			if (service == null)
			{
				throw new ArgumentNullException("service");
			}

			throw new InvalidOperationException(string.Format(
				CultureInfo.CurrentCulture, "Could not locate any instances of contract {0}.", key ?? service.Name));
		}

		protected override System.Collections.Generic.IEnumerable<object> GetAllInstances(Type service)
		{
			return this.Container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
		}

		protected override void BuildUp(object instance)
		{
			this.Container.InjectProperties(instance);
		}

		protected override IEnumerable<Assembly> SelectAssemblies()
		{
			return new[] { typeof(LandingPageViewModel).Assembly };
		}
	}
}
