using MyVote.BusinessObjects.Contracts;
using System.Collections.Generic;
using System.Threading;

namespace MyVote.BusinessObjects
{
	public sealed class ExecutionCallContext
		: ICallContext
	{
#if MOBILE
        private static readonly Dictionary<int, Dictionary<string, object>> context =
			new Dictionary<int, Dictionary<string, object>>();
#else
		private static readonly Dictionary<ExecutionContext, Dictionary<string, object>> context =
			 new Dictionary<ExecutionContext, Dictionary<string, object>>();
#endif

		public void FreeNamedDataSlot(string name)
		{
#if MOBILE
            var threadId = Thread.CurrentThread.ManagedThreadId;
		    if (ExecutionCallContext.context.ContainsKey(threadId))
		    {
		        var items = ExecutionCallContext.context[threadId];
		    	items.Remove(name);

		    	if(items.Count == 0)
		    	{
		    		ExecutionCallContext.context.Remove(threadId);
		    	}
		    }
#else
			var executionContext = ExecutionContext.Capture();

			if (ExecutionCallContext.context.ContainsKey(executionContext))
			{
				var items = ExecutionCallContext.context[executionContext];
				items.Remove(name);

				if (items.Count == 0)
				{
					ExecutionCallContext.context.Remove(executionContext);
				}
			}
#endif
		}

		public T GetData<T>(string name)
			where T: class
		{
#if MOBILE
            var threadId = Thread.CurrentThread.ManagedThreadId;

            if (!ExecutionCallContext.context.ContainsKey(threadId))
            {
            	ExecutionCallContext.context.Add(threadId, new Dictionary<string, object>());
            	return default(T);
            }
            else
            {
            	var items = ExecutionCallContext.context[threadId];

            	if(items.ContainsKey(name))
            	{
            		return (T)items[name];
            	}
            	else
            	{
            		return default(T);
            	}
            }
#else
			var executionContext = ExecutionContext.Capture();

			if (!ExecutionCallContext.context.ContainsKey(executionContext))
			{
				ExecutionCallContext.context.Add(executionContext, new Dictionary<string, object>());
				return default(T);
			}
			else
			{
				var items = ExecutionCallContext.context[executionContext];

				if (items.ContainsKey(name))
				{
					return (T)items[name];
				}
				else
				{
					return default(T);
				}
			}
#endif
		}

		public void SetData<T>(string name, T value)
			where T: class
		{
#if MOBILE
            var threadId = Thread.CurrentThread.ManagedThreadId;

            if (!ExecutionCallContext.context.ContainsKey(threadId))
            {
            	ExecutionCallContext.context.Add(threadId, new Dictionary<string, object>());
            }

            ExecutionCallContext.context[threadId][name] = value;
#else
			var executionContext = ExecutionContext.Capture();

			if (!ExecutionCallContext.context.ContainsKey(executionContext))
			{
				ExecutionCallContext.context.Add(executionContext, new Dictionary<string, object>());
			}

			ExecutionCallContext.context[executionContext][name] = value;
#endif
		}
	}
}