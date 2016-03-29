using MyVote.BusinessObjects.Contracts;
using System.Runtime.Remoting.Messaging;

namespace MyVote.BusinessObjects
{
	public sealed class ActivatorCallContext
		: ICallContext
	{
		public void FreeNamedDataSlot(string name)
		{
			CallContext.FreeNamedDataSlot(name);
		}

		public T GetData<T>(string name)
		{
			return (T)CallContext.GetData(name);
		}

		public void SetData<T>(string name, T value)
		{
			CallContext.SetData(name, value);
		}
	}
}