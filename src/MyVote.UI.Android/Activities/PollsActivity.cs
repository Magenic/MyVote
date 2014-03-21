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
using Android.Graphics;
using MyVote.UI.NavigationCriteria;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects;
using Csla.Axml.Binding;
using System.Threading.Tasks;
using MyVote.UI.ViewModels;
using MyVote.UI.Helpers;
using System.Collections.ObjectModel;
using UrlImageViewHelper;
using System.Threading;
using Csla;

namespace MyVote.UI.Droid
{
	[Actionbar]
    [Activity (Label = "View Polls", WindowSoftInputMode=SoftInput.StateHidden, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]			
	public class PollsActivity : MyVoteActivity
	{
		[Inject]
		public IObjectFactory<IPollSearchResults> objectFactory { get; set; }

        [Inject]
		public IObjectFactory<IPollDataResults> ResultsFactory { get; set; }

		private PollsPageSearchNavigationCriteria NavigationCriteria { get; set; }

		private IPollSearchResults PollSearchResults { get; set; }

		private List<IPollSearchResultsByCategory> PollSearchResultsByCatagory { get; set;}

		private PollSearchOptionViewModel SelectedSearchOption;

		private ObservableCollection<PollSearchOptionViewModel> SearchOptions { get;  set; }

		private BindingManager Bindings;

		//declare needed variables
		private bool mCanClickPoll;

		//declare needed layout variables
		private LinearLayout mCatagoryLayout;
		private Activity mActivityContext;
		private EditText mSearchText;

		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.pollview_new);

			mActivityContext = this;

			mCatagoryLayout = FindViewById<LinearLayout> (Resource.Id.pollview_catagoryLinearLayout);

			Spinner spinner = FindViewById<Spinner> (Resource.Id.pollTypeSpinner);
			spinner.ItemSelected += HandleSearchOptionSelected;
			ArrayAdapter adapter = ArrayAdapter.CreateFromResource (
				this, Resource.Array.poll_views, Android.Resource.Layout.SimpleSpinnerItem);

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;

			mSearchText = FindViewById<EditText> (Resource.Id.pollsPageSearchField);

			FindViewById<Button> (Resource.Id.searchButton).Click += SearchClicked;
			FindViewById<Button> (Resource.Id.addPollButton).Click += AddPollClicked;

			this.SearchOptions = new ObservableCollection<PollSearchOptionViewModel>();
			this.Bindings = new BindingManager (this);

			bool keywordSearch = ((NavigationCriteria != null) && (!string.IsNullOrEmpty(NavigationCriteria.SearchQuery)));
			this.PopulateFilterOptions(keywordSearch);

			this.NavigationCriteria = DeserializeNavigationCriteria<PollsPageSearchNavigationCriteria> ();

			FindViewById<EditText>(Resource.Id.pollsPageSearchField).TextChanged += HandleSearchTextChanged;

            await SearchPollsAsync();
		}

		protected override void OnStart()
		{
			base.OnStart();
		}

		protected override void OnResume()
		{
			base.OnResume();
			//NOTE - this should not be necessary - not sure why this is being repeated
			if (LoginActivity.LOG_OUT_OF_APP) {
				this.Finish ();
			}
		}

		protected override void OnPause()
		{
			base.OnPause ();
		}

		public override void OnBackPressed ()
		{
			base.OnBackPressed ();
			LoginActivity.LOG_OUT_OF_APP = true;
		}
		
		void HandleSearchTextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{
			this.NavigationCriteria.SearchQuery = (sender as EditText).Text;
		}

		async void SearchClicked (object sender, EventArgs e)
		{
			setDialogMessage("Retrieving poll search results...");
			await SearchPollsAsync ();
		}

		void setDialogMessage(string loadingMessage)
		{
			MyVoteActivity.setLoadingMessage(loadingMessage);
		}

		void HandleSearchOptionSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			this.SelectSearchOption (this.SearchOptions [e.Position]);
		}

		void AddPollClicked (object sender, EventArgs e)
		{
			var addPollPage = new Intent (this, typeof(AddPollActivity));
			StartActivity (addPollPage);
		}

		public async Task SearchPollsAsync()
		{
			try{

				this.IsBusy = true;
				this.PollSearchResults = null;

				if((NavigationCriteria == null) || (string.IsNullOrEmpty(NavigationCriteria.SearchQuery)))
					this.PollSearchResults = await this.objectFactory.FetchAsync(this.SelectedSearchOption.QueryType);
				else
					this.PollSearchResults = await this.objectFactory.FetchAsync(NavigationCriteria.SearchQuery);

				layoutActivity();
			
			}finally{

				this.IsBusy = false;
			}
		}

		public async Task<IPollDataResults> LoadPollAsync(IPollSearchResult poll)
        {
            setDialogMessage("Retrieving poll search results...");

            this.IsBusy = true;

			Task<IPollDataResults> pollResults = null;
            var hasError = false;
            try
            {
                 pollResults = this.ResultsFactory.FetchAsync(poll.Id);
            }
            catch (DataPortalException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                hasError = true;
            }
            this.IsBusy = false;

            if (hasError)
            {
                this.MessageBox.Show(this ,"There was an error loading the poll results. Please try again.", "Error");
            }
            return await pollResults;
        }

		public void ViewPoll(IPollSearchResult poll)
		{
			if (poll == null)
			{
				throw new ArgumentNullException("poll");
			}

			AnswerPollFragment votePollFragment = (AnswerPollFragment)
				FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);

            if (votePollFragment == null)
            {
                votePollFragment = new AnswerPollFragment(){
                    NavigationCriteria = new ViewPollPageNavigationCriteria(){
                        PollId = poll.Id
                    }
                };

                ((AnswerPollFragment)votePollFragment).PollAlreadyVoted += (sender, e) => this.ViewAnswer(poll);
                ((AnswerPollFragment)votePollFragment).ClosePressed += (sender, e) => 
                {
                    var removeTransaction = FragmentManager.BeginTransaction();
                    removeTransaction.Remove((Fragment)sender);
                    removeTransaction.Commit();
                };


                FragmentTransaction ft = FragmentManager.BeginTransaction();
                ft.SetTransition(FragmentTransit.FragmentOpen);
                ft.SetCustomAnimations(Resource.Animation.slide_in_right, Resource.Animation.slide_out_right, Resource.Animation.slide_in_right, Resource.Animation.slide_out_right);

				ft.Add(Resource.Id.polls_layout, votePollFragment);
                ft.Commit();
            }
		}


        public async void ViewAnswer(IPollSearchResult poll)
        {
            if (poll == null)
            {
                throw new ArgumentNullException("poll");
            }

            ViewPollFragment votePollFragment = (ViewPollFragment)
				FragmentManager.FindFragmentById(Resource.Id.fragmentContainer);

            if (votePollFragment == null)
            {
                var results = await this.LoadPollAsync(poll);

                votePollFragment = new ViewPollFragment(){
                    NavigationCriteria = new ViewPollPageNavigationCriteria(){
                        PollId = poll.Id
                    },
                    Results = results
                };
               

                ((ViewPollFragment)votePollFragment).ClosePressed += (sender, e) => 
                {
                    var removeTransaction = FragmentManager.BeginTransaction();
                    removeTransaction.Remove((Fragment)sender);
                    removeTransaction.Commit();
                };

                ((ViewPollFragment)votePollFragment).DeletePressed += async (sender, e) => 
                {
                    var removeTransaction = FragmentManager.BeginTransaction();
                    removeTransaction.Remove((Fragment)sender);
                    removeTransaction.Commit();
                    await this.SearchPollsAsync();
                };


				FragmentTransaction ft = FragmentManager.BeginTransaction();
                ft.SetCustomAnimations(Resource.Animation.slide_in_right, Resource.Animation.slide_out_right);

				ft.Replace(Resource.Id.polls_layout, votePollFragment);
                ft.Commit();
            }
        }

		public void SelectSearchOption(PollSearchOptionViewModel searchOption)
		{
			this.SelectedSearchOption = searchOption;

			bool queryMode = this.SelectedSearchOption.Display == "Keyword Search Result";

			mSearchText.Visibility = queryMode ? ViewStates.Visible : ViewStates.Invisible;
			if (!queryMode)
				mSearchText.Text = string.Empty;
		}

		private void PopulateFilterOptions(bool keywordSearch)
		{
			//if (!keywordSearch)
			{
				this.SearchOptions.Add(new PollSearchOptionViewModel
				                       {
					Display = "Most Popular",
					QueryType = PollSearchResultsQueryType.MostPopular
				});
				this.SearchOptions.Add(new PollSearchOptionViewModel
				                       {
					Display = "By Date (Newest First)",
					QueryType = PollSearchResultsQueryType.Newest
				});
			}
			//else
			{
				this.SearchOptions.Add(new PollSearchOptionViewModel
				                       {
					Display = "Keyword Search Result",
					QueryType = PollSearchResultsQueryType.Newest
				});
			}

			this.SelectedSearchOption = this.SearchOptions.Last();
		}

		private void ExecuteSearch()
		{
			var task = this.SearchPollsAsync();
			var awaiter = task.GetAwaiter();
			awaiter.OnCompleted(() =>
			                    {
				if (task.Exception != null)
				{
					System.Diagnostics.Debug.WriteLine(task.Exception.Message);
				}
			});
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			base.OnSaveInstanceState (outState);
		}

		public override Java.Lang.Object OnRetainNonConfigurationInstance ()
		{
			return new ResultsWrapper(){
				Results = this.PollSearchResults
			};
		}
				
		private class ResultsWrapper : Java.Lang.Object
		{
			public IPollSearchResults Results {get; set;}
		}

		protected override void OnRestoreInstanceState (Bundle savedInstanceState)
		{
			base.OnRestoreInstanceState (savedInstanceState);
			PollSearchResults = (LastNonConfigurationInstance as ResultsWrapper).Results;
			layoutActivity ();
		}

		/*
		 * lays out activity in the following manner:
		 * linear layout added for category title and count
		 * horizontal scroll view added for each poll view in the layout
		 * each poll is added as a view in the horizontal scroll view
		 */
		private void layoutActivity()
		{
			mCatagoryLayout.RemoveAllViews ();

			var pollsByCatagory = this.PollSearchResults.SearchResultsByCategory;
			foreach (IPollSearchResultsByCategory catagoryPolls in pollsByCatagory) 
			{
				mCatagoryLayout.AddView (getCatagoryTitleLayout(catagoryPolls.Category, catagoryPolls.SearchResults.Count));
				mCatagoryLayout.AddView (getCatagoryHorizontalScrollViewLayout(catagoryPolls));
			}
		}

		/*
		 * lays out then returns linear layout for catagory title and count
		 */
		private LinearLayout getCatagoryTitleLayout(String catagory, int pollCountInCatagory)
		{
			LinearLayout catagoryTitleLayout = (LinearLayout)mActivityContext.LayoutInflater.Inflate (Resource.Layout.pollview_catagorytitle, null);

			TextView catagoryTitleTextView = catagoryTitleLayout.FindViewById<TextView> (Resource.Id.pollview_catagoryTitle);
			catagoryTitleTextView.Text = catagory;

			TextView catagoryCountTextView = catagoryTitleLayout.FindViewById<TextView> (Resource.Id.pollview_catagoryCount);
			catagoryCountTextView.Text = pollCountInCatagory.ToString() + " Polls";

			return catagoryTitleLayout;
		}

		/*
		 * instanitates a horizontal scroll view then addes the specified catagories subviews to it
		 */
		private HorizontalScrollView getCatagoryHorizontalScrollViewLayout(IPollSearchResultsByCategory pollsByCatagory)
		{
			//set orientation and sizes
			HorizontalScrollView horizontalScrollView = new HorizontalScrollView (mActivityContext);
			HorizontalScrollView.LayoutParams horizontalLayoutParameters = new HorizontalScrollView.LayoutParams (
				HorizontalScrollView.LayoutParams.WrapContent, HorizontalScrollView.LayoutParams.WrapContent);
			horizontalScrollView.LayoutParameters = horizontalLayoutParameters;

			LinearLayout pollLinearLayout = new LinearLayout (mActivityContext);
			pollLinearLayout.Orientation = Orientation.Horizontal;
			LinearLayout.LayoutParams linearLayoutParameters = new LinearLayout.LayoutParams (
				LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
			pollLinearLayout.LayoutParameters = linearLayoutParameters;

			var polls = pollsByCatagory.SearchResults;
			foreach(IPollSearchResult poll in polls)
			{
				RelativeLayout pollLayout = (RelativeLayout)mActivityContext.LayoutInflater.Inflate (Resource.Layout.pollview_container, null);
				pollLayout.SetBackgroundColor (Color.Transparent);

				ImageView pollLayoutImage = pollLayout.FindViewById<ImageView> (Resource.Id.pollview_cellImage);
				if(poll.ImageLink.CompareTo("")!=0)
					pollLayoutImage.SetUrlDrawable (poll.ImageLink);

				TextView pollLayoutTitle = pollLayout.FindViewById<TextView> (Resource.Id.pollview_cellTitle);
				pollLayoutTitle.Text = poll.Question;

				TextView pollLayoutResponsesCountText = pollLayout.FindViewById<TextView> (Resource.Id.pollview_voteCounts);
				pollLayoutResponsesCountText.Text = poll.SubmissionCount.ToString() + " Responses";

				pollLayout.Click += delegate {
				    this.ViewPoll (poll);
				};

				pollLinearLayout.AddView(pollLayout);
			}

			horizontalScrollView.AddView (pollLinearLayout);

			return horizontalScrollView;
		}
	}
}

