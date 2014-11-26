using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    public interface IVmPageMappings
    {
        Dictionary<Type, Type> Mapings { get; }
        NavigationPage Navigation { get; }
    }
}