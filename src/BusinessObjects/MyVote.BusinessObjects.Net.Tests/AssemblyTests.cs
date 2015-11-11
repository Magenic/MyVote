using System.Diagnostics.CodeAnalysis;
using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyVote.BusinessObjects.Net.Tests
{
	[SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors")]
	[TestClass]
	public sealed class AssemblyTests
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			ApplicationContext.DataPortalActivator = new ObjectActivator(new ContainerBuilder().Build());
		}
	}
}
