using Autofac;

namespace MyVote.BusinessObjects.Core.Contracts
{
	internal interface IBusinessScope
	{
		ILifetimeScope Scope { get; set; }
	}
}
