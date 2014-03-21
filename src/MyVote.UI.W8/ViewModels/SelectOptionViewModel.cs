using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
	public class SelectOptionViewModel<T>
	{
		public string Display { get; set; }
		public T Value { get; set; }
	}
}
