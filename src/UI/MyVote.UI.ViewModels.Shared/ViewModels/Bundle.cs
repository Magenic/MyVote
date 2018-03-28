using System.Collections.Generic;

namespace MyVote.UI.ViewModels
{
	public class Bundle : IBundle
	{
		public Bundle()
            : this(new Dictionary<string, string>())
        {
		}

		public Bundle(IDictionary<string, string> data)
		{
			Data = data ?? new Dictionary<string, string>();
		}

		public IDictionary<string, string> Data { get; private set; }
	}
}