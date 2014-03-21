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
using MyVote.BusinessObjects.Contracts;

namespace MyVote.UI.Droid
{
	public class ActionbarAttribute : Attribute{}

	[Activity (Label = "MyVoteActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape )]			
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

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			_dialog = new ProgressDialog (this);
			_dialog.SetMessage(getLoadingMessage());
			_dialog.SetCancelable(true);
		}

		//methods used to store and retrieve shared preferences for application
		public const String APP_SHARED_PREF = "MyVote Shared Preferences";
		public const String APP_LOADING_MESSAGE = "Loading Message"; 
		private static IPollSearchResults mSearchResults;

		public static void setLoadingMessage(String loadingMessage)
		{
			var appPrefs = Application.Context.GetSharedPreferences(APP_SHARED_PREF, FileCreationMode.Private);  
			var appPrefEditor = appPrefs.Edit();
			appPrefEditor.PutString(APP_LOADING_MESSAGE, loadingMessage);
			appPrefEditor.Commit();
		}

		public static String getLoadingMessage()
		{
			var appPrefs = Application.Context.GetSharedPreferences(APP_SHARED_PREF, FileCreationMode.Private); 
			return appPrefs.GetString (APP_LOADING_MESSAGE, null);
		}

		//TODO - remove since this is not needed
		public static void setSearchResultsByCatagory(IPollSearchResults searchResults)
		{
			if (mSearchResults != null) {
				mSearchResults = null;
			}
			mSearchResults = searchResults;
		}

		public static IPollSearchResults getSearchResults()
		{
			return mSearchResults;
		}
	}
}

