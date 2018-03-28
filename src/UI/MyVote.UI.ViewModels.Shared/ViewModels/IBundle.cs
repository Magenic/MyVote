using System.Collections.Generic;

namespace MyVote.UI.ViewModels
{
    public interface IBundle
    {
		IDictionary<string, string> Data { get; }
	}
}
