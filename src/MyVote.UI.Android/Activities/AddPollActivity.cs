using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using Csla.Axml.Binding;
using Java.IO;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Services;
using MyVote.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyVote.UI.Droid
{
	[Actionbar]
    [Activity (Label = "Add a Poll", WindowSoftInputMode=SoftInput.StateHidden, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]			
	public class AddPollActivity : MyVoteActivity
	{
		[Inject]
		public IObjectFactory<IPoll> pollObjectFactory{ get; set; }

		[Inject]
		public IObjectFactory<IPollOption> pollOptionObjectFactory{ get; set; }

		[Inject]
		public IObjectFactory<ICategoryCollection> categoryObjectFactory{ get; set; }
	
        [Inject] 
        public IAzureStorageService AzureStorageService { get; set; }

        private IPoll _Poll;
		private IPoll Poll
        { 
            get
            {
                return _Poll;
            }
            set
            {
                if (this._Poll != null)
                {
                    this._Poll.PropertyChanged -= PollPropertyChanged;
                }
                this._Poll = value;

                if (value != null)
                {
                    value.PropertyChanged += PollPropertyChanged;
                }
            }
        }

        private void PollPropertyChanged(object sender, EventArgs e)
        {
            _SubmitButton.Enabled = this.CanSave;
        }

		private ObservableCollection<PollAnswerViewModel> PollAnswers {get; set;}

		private BindingManager Bindings;

		private bool SpecifyBeginEndDates {get; set;}

		private ICategoryCollection Categories { get; set; }

		//declare needed variables
		private File mPollPictureDirectory;

		//declare needed ui variables
		private Spinner _Spinner;
		private Button _SubmitButton;
		private ImageView _PollImageView;
		private TextView _StartDateLabel, _EndDateLabel;
		private RadioButton _SelectUseNoDateRadioButton, _SelectUseDateRadioButton;
        Button _StartDateButton;
        Button _EndDateButton;

        private List<int> _Answers;

		protected override async void OnCreate (Bundle bundle)
		{
            PollAnswers = new ObservableCollection<PollAnswerViewModel>();

			MyVoteActivity.setLoadingMessage ("Loading Add Poll interface...");
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.addpoll);

			_Spinner = FindViewById<Spinner> (Resource.Id.categorySpinner);

			_Spinner.ItemSelected += pollTypeSelected;;
			var adapter = ArrayAdapter.CreateFromResource (
				this, Resource.Array.poll_categories, Android.Resource.Layout.SimpleSpinnerItem);		

			adapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			_Spinner.Adapter = adapter;

			_SubmitButton = FindViewById<Button>(Resource.Id.postPollButton);
			_SubmitButton.Click += SubmitClicked;
			_SubmitButton.Enabled = false;

			_PollImageView = FindViewById<ImageView> (Resource.Id.newPollImage);
			_PollImageView.Click += GetPollImage;

			_SelectUseNoDateRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton1);
			_SelectUseNoDateRadioButton.Click += shouldHideDateVariables;

			_SelectUseDateRadioButton = FindViewById<RadioButton> (Resource.Id.radioButton2);
			_SelectUseDateRadioButton.Click += shouldHideDateVariables;

			_StartDateLabel = FindViewById<TextView> (Resource.Id.addpoll_startDateLabel);
			_StartDateButton = FindViewById<Button> (Resource.Id.addPollStartDateButton);

			_EndDateLabel = FindViewById<TextView> (Resource.Id.addpoll_endDateLabel);
            _EndDateButton = FindViewById<Button> (Resource.Id.addPollEndDateButton);

            _StartDateButton.Click += (sender, e) =>
            {
                ShowDialog(0);
            };

            _EndDateButton.Click += (sender, e) =>
            {
                ShowDialog(1);
            };


            _Answers = new List<int>()
            {
                Resource.Id.answerA,
                Resource.Id.answerB,
                Resource.Id.answerC,
                Resource.Id.answerD,
                Resource.Id.answerE,
            };

            foreach (var id in _Answers)
            {
                FindViewById<EditText>(id).TextChanged += HandleTextChanged;
            }

			if (IsThereAnAppToTakePictures ()) {
				CreateDirectoryForPictures ();
			}

            this.Bindings = new BindingManager (this);

            await this.CreatePollAsync();

            this.Poll.PollMinAnswers = 1;
            this.Poll.PollMaxAnswers = 1;
            SpecifyDates = false;

            this.SetupAnswers();

            await this.LoadCategoriesAsync();
            this.Poll.PollCategoryID = this.Categories[this._Spinner.SelectedItemPosition].ID;
            FindViewById<EditText>(Resource.Id.description).TextChanged += (sender, e) => this.Poll.PollQuestion = e.Text.ToString();
            toggleDateControls (false);
		}

        protected override Dialog OnCreateDialog (int id)
        {
            var dateToUse = id == 0 ? this.Poll.PollStartDate : this.Poll.PollEndDate;

            if (dateToUse.HasValue)
            {
                // Ugh, wish I could specify event args or something.
                if(id == 0)
                    return new DatePickerDialog(this, HandleStartDate, dateToUse.Value.Year, dateToUse.Value.Month, dateToUse.Value.Day); 
                else
                    return new DatePickerDialog(this, HandleEndDate, dateToUse.Value.Year, dateToUse.Value.Month, dateToUse.Value.Day); 

            }else{
                if(id == 0)
                    return new DatePickerDialog(this, HandleStartDate, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day); 
                else
                    return new DatePickerDialog(this, HandleEndDate, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day); 
            }
        }

        void HandleStartDate (object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.Poll.PollStartDate = e.Date;
            _StartDateButton.Text = this.Poll.PollStartDate.Value.ToString ("d");
        }

        void HandleEndDate (object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.Poll.PollEndDate = e.Date;
            _EndDateButton.Text = this.Poll.PollStartDate.Value.ToString ("d");
        }

		protected override void OnStart()
		{
			base.OnStart ();
		}

		protected override void OnResume()
		{
			base.OnResume ();
		}

		protected override void OnPause()
		{
			base.OnPause ();
		}

		protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
                return;

            _PollImageView.SetImageURI (data.Data);
            var bitmap = (_PollImageView.Drawable as BitmapDrawable).Bitmap;
            float factor = _PollImageView.Width / (float) bitmap.Width;
            var scaled = Bitmap.CreateScaledBitmap(bitmap, _PollImageView.Width, (int)(bitmap.Height * factor), true);
			_PollImageView.SetImageBitmap(scaled);

            var vm = new AndroidPollImageViewModel(this.AzureStorageService);
            Poll.PollImageLink = await vm.UploadImage(scaled);
		}

		async void HandleTextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{
            if (PollAnswers.Count() == 0)
                return;

            var id = _Answers.IndexOf((sender as EditText).Id);
            IsBusy = true;
            PollAnswers[id].PollAnswer = ((EditText)sender).Text;
            IsBusy = false;
            _SubmitButton.Enabled = CanSave; 
		}

		void shouldHideDateVariables (object sender, EventArgs e)
		{
            var dateEnabled = (sender as RadioButton).Id == Resource.Id.radioButton1;

			if (dateEnabled)
				toggleDateControls (false);
			else
				toggleDateControls (true);     

		}

        private bool _specifyDates = false;
        private bool SpecifyDates
        {
            get{
                return _specifyDates;
            }
            set{
                _specifyDates = value;

                if (value)
                {
                    Poll.PollStartDate = DateTime.Today;
                    Poll.PollEndDate = DateTime.Today.AddDays(7);
                }
                else
                {
                    Poll.PollStartDate = DateTime.Today;
                    Poll.PollEndDate = DateTime.MaxValue;
                }
            }
        }

		private void toggleDateControls(bool shouldBeVisible)
		{
			if (shouldBeVisible) {
				_StartDateLabel.Visibility = ViewStates.Visible;
				_StartDateButton.Visibility = ViewStates.Visible;
				_EndDateLabel.Visibility = ViewStates.Visible;
				_EndDateButton.Visibility = ViewStates.Visible;
			} else {
				_StartDateLabel.Visibility = ViewStates.Gone;
				_StartDateButton.Visibility = ViewStates.Gone;
				_EndDateLabel.Visibility = ViewStates.Gone;
				_EndDateButton.Visibility = ViewStates.Gone;
			}

			SpecifyDates = shouldBeVisible;
		}

		private void InitializeBindings(IPoll model)
		{
			this.Poll = model;
			this.Bindings.RemoveAll();
		}

		public async Task CreatePollAsync()
		{
			var identity = Csla.ApplicationContext.User.Identity as IUserIdentity;
			if (identity != null) this.Poll = await this.pollObjectFactory.CreateAsync(identity.UserID);
		}

		private void SetupAnswers()
		{
			for (int i = 0; i <= 5; i++)
				PollAnswers.Add(new PollAnswerViewModel(this.Poll, this.pollOptionObjectFactory, (short)i));
		}

		public async Task LoadCategoriesAsync()
		{
			this.Categories = await this.categoryObjectFactory.FetchAsync();
		}

		private void SubmitClicked (object sender, EventArgs e)
		{
			this.AsyncSubmitClicked ();
		}

		private void GetPollImage (object sender, EventArgs e)
		{
			Intent imageIntent = new Intent ();
			imageIntent.SetType ("image/*");
			imageIntent.SetAction (Intent.ActionGetContent);
			StartActivityForResult (
				Intent.CreateChooser (imageIntent, "Select photo"), 0);
		}

		private async Task AsyncSubmitClicked()
		{
			this.Poll = await this.Poll.SaveAsync () as IPoll;

            Finish();
			
		}

		void pollTypeSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{

		}

		private bool IsThereAnAppToTakePictures()
		{
		    Intent intent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
		    IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
		    return availableActivities != null && availableActivities.Count > 0;
		}

		private void CreateDirectoryForPictures()
		{
		    mPollPictureDirectory = 
				new File(Android.OS.Environment.
				         GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "MyVotePollImages");
		    if (!mPollPictureDirectory.Exists())
		    {
		        mPollPictureDirectory.Mkdirs();
		    }
		}

        public bool CanSave
        {
            get { return Poll != null && Poll.IsSavable; }
        }
	}
}

