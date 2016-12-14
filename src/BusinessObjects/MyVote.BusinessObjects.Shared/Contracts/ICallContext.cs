namespace MyVote.BusinessObjects.Contracts
{
	public interface ICallContext
	{
		void FreeNamedDataSlot(string name);
		T GetData<T>(string name)
			where T: class;
		void SetData<T>(string name, T value)
			where T: class;
	}
}
