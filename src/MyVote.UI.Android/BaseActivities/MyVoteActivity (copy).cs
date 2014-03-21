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
using MyVote.UI.Helpers;
using System.Reflection;

namespace MyVote.UI.Droid
{
	public class ActionbarAttribute : Attribute{}


	[Activity (Label = "MyVoteActivity")]			
	public class MyVoteActivity : AutofacActivity
	{

		[Inject]
		public IMessageBox MessageBox { get; set;}

		private ProgressDialog _dialog;

		private static string NAVIGATION_CRITERIA = "NAVIGATION_CRITERIA";

		protected T DeserializeNavigationCriteria<T>() where T: class
		{
			return Serializer.Deserialize<T> (Intent.GetStringExtra (NAVIGATION_CRITERIA) ?? "{}") ?? default(T);
		}

		protected void SerializeNavigationCriteria<T>(Intent intent, T criteria)
		{
			intent.PutExtra (NAVIGATION_CRITERIA, Serializer.Serialize (criteria));
		}

		private bool _IsBusy;
		protected bool IsBusy {
			get{
				return _IsBusy;
			}
			set{
				_IsBusy = value;
				if(_IsBusy)
				{
					_dialog.Show ();
				}
				else{
					_dialog.Hide ();
				}
			}
		}


		public void SetupActionBar()
		{
			if (!this.GetType ().GetCustomAttributes (typeof(ActionbarAttribute), false).Any ()) {
				this.ActionBar.Hide ();
			} else {
				this.ActionBar.Show ();
				this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
				var tab1 = this.ActionBar.NewTab ();
				tab1.SetText ("Hello");
				var tab2 = this.ActionBar.NewTab ();
				tab2.SetText ("Goodbye");
			}
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			_dialog = new ProgressDialog (this);
			_dialog.SetMessage("Hakuna matata");
			_dialog.SetCancelable(true);
			SetupActionBar ();
			//var actionBar = this.ActionBar.NewTab ();
			//actionBar.SetText("Hello");
		}


	}
}

