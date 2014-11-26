using System;

namespace MyVote.BusinessObjects.Attributes
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
	internal sealed class DependencyAttribute
		: Attribute { }
}
