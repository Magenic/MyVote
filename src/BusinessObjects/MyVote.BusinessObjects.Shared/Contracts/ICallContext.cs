namespace MyVote.BusinessObjects.Contracts
{
	public interface ICallContext
	{
		void FreeNamedDataSlot(string name);
		T GetData<T>(string name);
		void SetData<T>(string name, T value);
	}
}
