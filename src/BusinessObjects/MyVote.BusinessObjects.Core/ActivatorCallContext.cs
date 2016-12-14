using System.Collections.Generic;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.BusinessObjects
{
	public sealed class ActivatorCallContext
		: ICallContext
	{
		// Note: If any of the BOs ever do any async operations,
		// this will probably fail.
		private readonly IDictionary<string, object> values =
			new Dictionary<string, object>();

		public void FreeNamedDataSlot(string name)
		{
			this.values.Remove(name);
		}

		public T GetData<T>(string name)
			where T: class
		{
			return this.values.ContainsKey(name) ? 
				this.values[name] as T : null as T;
		}

		public void SetData<T>(string name, T value)
			where T: class
		{
			this.values.Add(name, value);
		}
	}
}