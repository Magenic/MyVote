using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface ICategory
		: IReadOnlyBaseCore
	{
		int ID { get; }
		string Name { get; }
	}
}
