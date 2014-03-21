using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IPollOption
		: IBusinessBaseCore
	{
		int? PollOptionID { get; }
		int? PollID { get; }
		short? OptionPosition { get; set; }
		string OptionText { get; set; }
	}
}
