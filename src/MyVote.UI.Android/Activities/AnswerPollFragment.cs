using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.NavigationCriteria;
using MyVote.BusinessObjects;
using System.Threading.Tasks;
using Autofac;
using Csla;

namespace MyVote.UI.Droid
{
    public class AnswerPollFragment : MyVoteFragment
    {
     
        [Inject]
		public IObjectFactory<IPoll> PollFactory { get; set;}

        [Inject]
		public IObjectFactory<IPollSubmissionCommand> CommandFactory { get; set; }

        private IPollSubmission _PollSubmission;

        private Button _SubmitButton;

        public event EventHandler PollAlreadyVoted;

        public event EventHandler ClosePressed;


        private ListView _PollListView;
        public ViewPollPageNavigationCriteria NavigationCriteria { get; set; }
        public  override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);
        }

        private Context _hack;
        private LayoutInflater _inflater;
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			MyVoteActivity.setLoadingMessage ("Loading answers for poll...");
            base.OnCreateView(inflater, container, savedInstanceState);

            _hack = container.Context;
            _inflater = inflater;
            var view = inflater.Inflate(Resource.Layout.viewpoll, container, false);

			this.PollFactory = AutofacInject.Container.Resolve<IObjectFactory<IPoll>> ();

            _PollListView = view.FindViewById<ListView> (Resource.Id.viewPollResponses);

            _SubmitButton = view.FindViewById<Button> (Resource.Id.submitVoteButtonn);
            _SubmitButton.Click += async (sender, e) => {
                await Submit();
            };

            var closeButton = view.FindViewById<Button> (Resource.Id.closeVoteButtonn);
            closeButton.Click += (sender, e) => {
                this.ClosePressed.SafeInvoke(this, EventArgs.Empty);
            };

            // Disabled by default.
            _SubmitButton.Enabled = false;

            LoadPollAsync ();

            return view;
        }

        public async Task LoadPollAsync()
        {
            this.IsBusy = true;

            var hasError = false;
            try
            {
                var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;

				var command = await this.CommandFactory.CreateAsync();
				command.PollID = this.NavigationCriteria.PollId;
				command.UserID = identity.UserID.Value;
				command = await this.CommandFactory.ExecuteAsync(command);

                if (command.Submission != null)
                {
                    this._PollSubmission = command.Submission;

                    _PollListView.ChoiceMode = ChoiceMode.Multiple;
                    _PollListView.Adapter = new PollAnswersAdapter (_hack, Android.Resource.Layout.SimpleListItemChecked, this._PollSubmission.Responses, _inflater);
                    _PollListView.ItemClick += (sender, e) => {
                        _PollSubmission.Responses[(int)e.Id].IsOptionSelected = ((CheckedTextView)e.View).Checked;
                        _SubmitButton.Enabled = CanSubmit;
                    };
                }
                else
                {
                    // The user has already taken the poll. Show them the results instead.

                    PollAlreadyVoted.SafeInvoke(this, EventArgs.Empty);
                }
            }
            catch (DataPortalException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                hasError = true;
            }
            this.IsBusy = false;

            if (hasError)
            {
                this.MessageBox.Show(_hack,"There was an error loading the poll. Please try again.", "Error");
            }
        }

        public async Task Submit()
        {
            this.IsBusy = true;

            var hasError = false;
            try
            {
                this._PollSubmission = await this._PollSubmission.SaveAsync() as IPollSubmission;
            }
            catch (DataPortalException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                hasError = true;
            }
            this.IsBusy = false;

            if (!hasError)
            {
                this.PollAlreadyVoted.SafeInvoke(this, EventArgs.Empty);
            }
            else
            {
                this.MessageBox.Show(_hack, "There was an error submitting your poll. Please try again.", "Error");
            }
        }

        public async Task DeletePoll()
        {
            bool? result = true;//this.MessageBox.Show(this, "Are you sure you want to delete this poll?", "Delete Poll?", MessageBoxButtons.YesNo);

            if (result != null && result.Value)
            {
                var poll = await this.PollFactory.FetchAsync(this._PollSubmission.PollID);
                poll.Delete();
                await poll.SaveAsync();

                //this.Navigation.GoBack();
            }
        }

        public bool CanSubmit
        {
            get
            {
                if (this._PollSubmission != null)
                {
                    var numberSelectedResponses = this._PollSubmission.Responses.Count(r => r.IsOptionSelected);
                    return numberSelectedResponses >= this._PollSubmission.PollMinAnswers
                        && numberSelectedResponses <= this._PollSubmission.PollMaxAnswers;
                }
                else
                {
                    return false;
                }
            }
        }


        private class PollAnswersAdapter : ArrayAdapter<IPollSubmissionResponse>
        {
            private List<IPollSubmissionResponse> _Polls { get; set;}
            private LayoutInflater _inflator;

            public PollAnswersAdapter(Context context, int viewResourceId, IPollSubmissionResponseCollection results, LayoutInflater inflater)
                : base(context, viewResourceId, results.ToList())
            {
                _inflator = inflater;
                _Polls = results.ToList();

            }

            public override View GetView (int position, View convertView, ViewGroup parent)
            {
                var view = convertView;
                if(view == null){
                    // TODO - What's the point of passing the layout in the constructor?
                    view = _inflator.Inflate (Android.Resource.Layout.SimpleListItemChecked, null);
                }
                var item = GetItem (position);

                view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = item.OptionText;
                return view;
            }


        }
    }
}

