using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Reflection;
using Autofac;
using MyVote.BusinessObjects;

namespace MyVote.UI.Droid
{
	public class AutofacActivity : Activity
	{
		private static ContainerBuilder ContainerBuilder { get; set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate (bundle);
			this.ResolveAutofacDependencies ();
		}
	}
}

