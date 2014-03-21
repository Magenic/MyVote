using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spackle.Extensions;

namespace MyVote.Core.Tests
{
	[TestClass]
	public sealed class SystemTimeTests
	{
		[TestMethod]
		public void CallNow()
		{
			var now = DateTime.Now.AddYears(-2);
			var myNow = new Func<DateTime>(() => now);

			using (myNow.Bind(() => SystemTime.UtcNow))
			{
				Assert.AreEqual(now, SystemTime.UtcNow());
			}

			Assert.AreNotEqual(now, SystemTime.UtcNow());
		}
	}
}
