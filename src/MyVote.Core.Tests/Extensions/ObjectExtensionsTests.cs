using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.Core.Extensions;

namespace MyVote.Core.Tests.Extensions
{
	[TestClass]
	public sealed class ObjectExtensionsTests
	{
		[TestMethod]
		public void GetPropertyName()
		{
			Assert.AreEqual("MyProperty", this.GetPropertyName(_ => _.MyProperty));
		}

		public string MyProperty { get; private set; }
	}
}
