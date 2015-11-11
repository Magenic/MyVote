using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.BusinessObjects.Extensions;
using MyVote.BusinessObjects.Net.Tests.Extensions.Contracts;

namespace MyVote.BusinessObjects.Net.Tests.Extensions
{
	[TestClass]
	public sealed class TypeExtensionsTests
	{
		[TestMethod]
		public void GetConcreteTypeWhenTargetIsNull()
		{
			Assert.IsNull((null as Type).GetConcreteType());
		}

		[TestMethod]
		public void GetConcreteTypeWhenTargetIsAnInterface()
		{
			var target = typeof(ITarget);
			Assert.AreSame(typeof(Target), target.GetConcreteType());
		}

		[TestMethod]
		public void GetConcreteTypeWhenTargetIsAClass()
		{
			var target = typeof(Target);
			Assert.AreSame(target, target.GetConcreteType());
		}

		[TestMethod]
		public void GetConcreteTypeWhenTargetHasNoNamespace()
		{
			var target = typeof(IHaveNoNamespace);
			Assert.AreSame(target, target.GetConcreteType());
		}
	}

	public class Target : ITarget { }
}

[SuppressMessage("Microsoft.Design", "CA1050:DeclareTypesInNamespaces")]
[SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
public interface IHaveNoNamespace { }

namespace MyVote.BusinessObjects.Net.Tests.Extensions.Contracts
{
	[SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
	public interface ITarget { }
}
