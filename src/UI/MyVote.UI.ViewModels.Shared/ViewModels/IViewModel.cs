using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
    public interface IViewModel
    {
		void Init(object parameter);

		void Start();
	}
}
