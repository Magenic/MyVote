using Autofac;
using System;
using System.Linq;
using System.Reflection;

#if MOBILE
using Xamarin.Forms;
#else
using Windows.UI.Xaml.Controls;
using Csla.Reflection;
#endif // MOBILE

namespace MyVote.UI.Helpers
{
	public static class AutofacInject
	{
		public static IContainer Container { get; private set; }

		public static void ResolveAutofacDependencies(this object obj)
		{
			// Bootstrap

			if (Container == null)
			{
				Container = new Bootstrapper().Bootstrap();
			}

			if (obj is Page)
			{
				var builder = new ContainerBuilder();
				builder.RegisterInstance(obj).As<Page>();
				builder.Update(Container);
			}

			PropertyInfo[] properties =
				obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			using (var lifetimeScope = Container.BeginLifetimeScope())
			{
				foreach (var property in properties.Where(p => p.GetCustomAttributes(typeof(InjectAttribute), false).Any()))
				{
					object instance = null;
					if (!lifetimeScope.TryResolve(property.PropertyType, out instance))
					{
						throw new InvalidOperationException("Could not resolve type " + property.PropertyType);
					}
					property.SetValue(obj, instance);
				}
			}
		}
	}
}
