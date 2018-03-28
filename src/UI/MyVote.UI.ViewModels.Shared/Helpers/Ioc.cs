using Autofac;

namespace MyVote.UI.Helpers
{
    public static class Ioc
    {
		public static IContainer Container { get; set; }

		public static T Resolve<T>()
		{
			return Container.Resolve<T>();
		}

		public static object Resolve(System.Type objectType)
		{
			return Container.Resolve(objectType);
		}
	}
}
