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
	public class MyVoteFragment : Fragment
	{
		[Inject]
		public IMessageBox MessageBox { get; set;}

		private ProgressDialog _dialog;

		private static string NAVIGATION_CRITERIA = "NAVIGATION_CRITERIA";

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


		public override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
            this.ResolveAutofacDependencies ();
		}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var v = base.OnCreateView(inflater, container, savedInstanceState);
            _dialog = new ProgressDialog (container.Context);
            _dialog.SetMessage(MyVoteActivity.getLoadingMessage());
            _dialog.SetCancelable(true);
            return v;
        }

	}
}

