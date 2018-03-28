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
		public static void ResolveAutofacDependencies(this object obj)
		{
			// Bootstrap

			if (Ioc.Container == null)
			{
				Ioc.Container = new Bootstrapper().Bootstrap();
			}

			if (obj is Page)
			{
				var builder = new ContainerBuilder();
				builder.RegisterInstance(obj).As<Page>();
				builder.Update(Ioc.Container);
			}

			PropertyInfo[] properties =
				obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

			using (var lifetimeScope = Ioc.Container.BeginLifetimeScope())
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
