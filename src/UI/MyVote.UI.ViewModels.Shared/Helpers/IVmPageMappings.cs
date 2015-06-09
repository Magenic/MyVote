using System;
using System.Collections.Generic;

namespace MyVote.UI.Helpers
{
    public interface IVmPageMappings
    {
		Dictionary<Type, Type> Mappings { get; }
    }
}
