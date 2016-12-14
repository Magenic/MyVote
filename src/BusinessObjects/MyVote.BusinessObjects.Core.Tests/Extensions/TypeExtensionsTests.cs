using System;
using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Extensions;
using MyVote.BusinessObjects.Tests.Extensions.Contracts;
using Xunit;
using FluentAssertions;

namespace MyVote.BusinessObjects.Tests.Extensions
{
	public sealed class TypeExtensionsTests
	{
		[Fact]
		public void GetConcreteTypeWhenTargetIsNull()
		{
			(null as Type).GetConcreteType().Should().BeNull();
		}

		[Fact]
		public void GetConcreteTypeWhenTargetIsAnInterface()
		{
			typeof(ITarget).GetConcreteType().Should().BeSameAs(typeof(Target));
		}

		[Fact]
		public void GetConcreteTypeWhenTargetIsAClass()
		{
			typeof(Target).GetConcreteType().Should().BeSameAs(typeof(Target));
		}

		[Fact]
		public void GetConcreteTypeWhenTargetHasNoNamespace()
		{
			typeof(IHaveNoNamespace).GetConcreteType().Should().BeSameAs(typeof(IHaveNoNamespace));
		}
	}

	public class Target : ITarget { }
}

[SuppressMessage("Microsoft.Design", "CA1050:DeclareTypesInNamespaces")]
[SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
public interface IHaveNoNamespace { }

namespace MyVote.BusinessObjects.Tests.Extensions.Contracts
{
	[SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
	public interface ITarget { }
}
