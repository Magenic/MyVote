using Cirrious.MvvmCross.ViewModels;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;
using System;

namespace MyVote.UI
{
	public class MyVoteApp : MvxApplication
	{
		public override void Initialize()
		{
			RegisterAppStart(new MvxAppStart());
		}
	}
}
