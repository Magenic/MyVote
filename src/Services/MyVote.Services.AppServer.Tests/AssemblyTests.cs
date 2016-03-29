using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.BusinessObjects;
using System.Diagnostics.CodeAnalysis;

namespace MyVote.Services.AppServer.Tests
{
	[SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
	[TestClass]
	public sealed class AssemblyTests
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			ApplicationContext.DataPortalActivator = new ObjectActivator(
				new ContainerBuilder().Build(), new ActivatorCallContext());
		}
	}
}
