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
using System.Threading.Tasks;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using Csla;
using MyVote.UI.NavigationCriteria;
using Csla.Axml.Binding;
using MyVote.UI.Helpers;
using MyVote.BusinessObjects;

namespace MyVote.UI.Droid
{
    [Activity (Label = "RegistrationActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]			
	public class RegistrationActivity : MyVoteActivity
	{
		[Inject]
		public IObjectFactory<IUser> ObjectFactory {get; set;}

		[Inject]
		public IObjectFactory<IUserIdentity> UserIdentityObjectFactory {get; set;}

		private RegistrationPageNavigationCriteria NavigationCriteria { get; set; }

		private IUser User;

		private BindingManager Bindings;

        private Button _SubmitButton;

        Button _date;

		protected override async void OnCreate (Bundle bundle)
		{
			MyVoteActivity.setLoadingMessage ("Loading User Registration interface...");
			base.OnCreate (bundle);

			this.NavigationCriteria = new RegistrationPageNavigationCriteria ();
			this.NavigationCriteria.PollId = Intent.GetIntExtra ("NavigationCriteria_PollId", 0);
			this.NavigationCriteria.ProfileId = Intent.GetStringExtra ("NavigationCriteria_ProfileId");

			this.Bindings = new BindingManager (this);

			SetContentView(Resource.Layout.registration);

			_SubmitButton = FindViewById<Button>(Resource.Id.SubmitRegistrationButton);
			_SubmitButton.Click += SubmitRegistrationClicked;
            _SubmitButton.Enabled = false;


            FindViewById(Resource.Id.registration_sex).Click += HandleClick;
            _date = FindViewById<Button>(Resource.Id.registration_dob);
            _date.Click += (sender, e) =>
            {
                ShowDialog(0);
            };


			// OnInitialize isn't async, so we have to go old school.
			await this.CreateUserAsync();

            _date.Text = !this.User.BirthDate.HasValue ? "Enter birthdate" : this.User.BirthDate.Value.ToString ("d");  

			this.InitializeBindings (this.User);

		}

        protected override Dialog OnCreateDialog (int id)
        {
            if (this.User.BirthDate.HasValue)
            {
                return new DatePickerDialog(this, HandleDateSet, this.User.BirthDate.Value.Year, this.User.BirthDate.Value.Month, this.User.BirthDate.Value.Day); 
            }else{
                return new DatePickerDialog(this, HandleDateSet, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day); 
            }
        }

        void HandleDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
        {
            this.User.BirthDate = e.Date;
            _date.Text = this.User.BirthDate.Value.ToString ("d");
        }

       void HandleClick (object sender, EventArgs e)
       {
            this.User.Gender = (sender as RadioButton).Id == Resource.Id.radioMale ? "Male" : "Female"; 
       }

		private void InitializeBindings(IUser model)
		{
			this.User = model;
			this.Bindings.RemoveAll();
			Bindings.Add(Resource.Id.registration_username, "Text", this.User, "UserName");
			Bindings.Add(Resource.Id.registration_email, "Text", this.User, "EmailAddress");
			//Bindings.Add(Resource.Id.registration_sex, "Text", this.User, "Gender");
			Bindings.Add(Resource.Id.registration_zip, "Text", this.User, "PostalCode");
            this.User.PropertyChanged += HandlePropertyChanged;
		}

        void HandlePropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _SubmitButton.Enabled = User.IsSavable;
        }

		public async Task CreateUserAsync()
		{
			this.User = await this.ObjectFactory.CreateAsync(NavigationCriteria.ProfileId);
		}

		void SubmitRegistrationClicked (object sender, EventArgs e)
		{
			var t = this.Submit ();
			if (t.Exception != null)
				throw t.Exception;
		}

		public async Task Submit()
		{

			this.IsBusy = true;

            var hasError = false;
		    DateTime birthDate;
            hasError = !DateTime.TryParse(FindViewById<Button>(Resource.Id.registration_dob).Text, out birthDate);

		    if (!hasError)
		    {
		        this.User.BirthDate = birthDate;
		        try
		        {
		            Bindings.UpdateSourceForLastView();

		            this.User = await this.User.SaveAsync() as IUser;

		            InitializeBindings(this.User);

		            var identity = await this.UserIdentityObjectFactory.FetchAsync(this.User.ProfileID);

		            var principal = new CslaPrincipalCore(identity);
		            Csla.ApplicationContext.User = principal;
		        }
		        catch (DataPortalException ex)
		        {
		            System.Diagnostics.Debug.WriteLine(ex);
		            hasError = true;
		        }
		    }
		    this.IsBusy = false;

			if (!hasError)
			{

				// Always navigate to Polls page first
				var polls = new Intent (this, typeof(PollsActivity));
				StartActivity (polls);

				// If there is a PollId, the user is coming in from a URI link.
				// Navigate to the View Poll page, leaving the Polls page in the back
				// stack so the user can back up to it.
				if (this.NavigationCriteria.PollId.HasValue)
				{
					var criteria = new ViewPollPageNavigationCriteria
					{
						PollId = this.NavigationCriteria.PollId.Value
					};

					var registration = new Intent (this, typeof(ViewPollFragment));
					registration.PutExtra("PollId", criteria.PollId);
					StartActivity (registration);

					//this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
				}

			}
			else
			{
				this.MessageBox.Show(this, "There was an error saving your profile. Please try again.", "Error");
			}

		}
	}
}

