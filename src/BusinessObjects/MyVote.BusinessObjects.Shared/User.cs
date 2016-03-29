using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !MOBILE
using MyVote.Data.Entities;
using Csla.Data;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
#if ANDROID
	 [Android.Runtime.Preserve(AllMembers=true)]
#endif
	internal sealed class User
	  : BusinessBaseCore<User>, IUser
	{
		[RunLocal]
		private void DataPortal_Create(string profileId)
		{
			this.ProfileID = profileId;
			this.BusinessRules.CheckRules();
		}

#if !NETFX_CORE && !MOBILE
		private void DataPortal_Fetch(string profileId)
		{
			using (this.BypassPropertyChecks)
			{
				var entity = this.Entities.MVUsers.Where(_ => _.ProfileID == profileId).First();
				this.ProfileID = entity.ProfileID;
				this.BirthDate = entity.BirthDate;
				this.EmailAddress = entity.EmailAddress;
				this.PostalCode = entity.PostalCode;
				this.UserID = entity.UserID;
				this.UserRoleID = entity.UserRoleID;
				this.UserName = entity.UserName;
				this.LastName = entity.LastName;
				this.FirstName = entity.FirstName;
				this.Gender = entity.Gender;
			}
		}

		protected override void DataPortal_Insert()
		{
			var entity = new MVUser();
			DataMapper.Map(this, entity, nameof(this.Entities));
			this.Entities.MVUsers.Add(entity);
			this.Entities.SaveChanges();
			this.UserID = entity.UserID;
		}

		protected override void DataPortal_Update()
		{
			var entity = this.Entities.MVUsers.Where(_ => _.ProfileID == this.ProfileID).First();
			DataMapper.Map(this, entity, nameof(this.Entities));
			this.Entities.SaveChanges();
			this.UserID = entity.UserID;
		}
#endif

		public static readonly PropertyInfo<DateTime?> BirthDateProperty =
		  User.RegisterProperty<DateTime?>(_ => _.BirthDate);
		public DateTime? BirthDate
		{
			get { return this.GetProperty(User.BirthDateProperty); }
			set { this.SetProperty(User.BirthDateProperty, value); }
		}

		public static readonly PropertyInfo<string> EmailAddressProperty =
		  User.RegisterProperty<string>(_ => _.EmailAddress);
		[Required(ErrorMessage = "The e-mail address is required.")]
		public string EmailAddress
		{
			get { return this.GetProperty(User.EmailAddressProperty); }
			set { this.SetProperty(User.EmailAddressProperty, value); }
		}

		public static readonly PropertyInfo<string> FirstNameProperty =
		  User.RegisterProperty<string>(_ => _.FirstName);
		public string FirstName
		{
			get { return this.GetProperty(User.FirstNameProperty); }
			set { this.SetProperty(User.FirstNameProperty, value); }
		}

		public static readonly PropertyInfo<string> GenderProperty =
		  User.RegisterProperty<string>(_ => _.Gender);
		public string Gender
		{
			get { return this.GetProperty(User.GenderProperty); }
			set { this.SetProperty(User.GenderProperty, value); }
		}

		public static readonly PropertyInfo<string> LastNameProperty =
		  User.RegisterProperty<string>(_ => _.LastName);
		public string LastName
		{
			get { return this.GetProperty(User.LastNameProperty); }
			set { this.SetProperty(User.LastNameProperty, value); }
		}

		public static readonly PropertyInfo<string> PostalCodeProperty =
		  User.RegisterProperty<string>(_ => _.PostalCode);
		public string PostalCode
		{
			get { return this.GetProperty(User.PostalCodeProperty); }
			set { this.SetProperty(User.PostalCodeProperty, value); }
		}

		public static readonly PropertyInfo<string> ProfileAuthTokenProperty =
		  User.RegisterProperty<string>(_ => _.ProfileAuthToken);
		public string ProfileAuthToken
		{
			get { return this.GetProperty(User.ProfileAuthTokenProperty); }
			set { this.SetProperty(User.ProfileAuthTokenProperty, value); }
		}

		public static readonly PropertyInfo<string> ProfileIDProperty =
		  User.RegisterProperty<string>(_ => _.ProfileID);
		public string ProfileID
		{
			get { return this.ReadProperty(User.ProfileIDProperty); }
			private set { this.LoadProperty(User.ProfileIDProperty, value); }
		}

		public static readonly PropertyInfo<int?> UserIDProperty =
		  User.RegisterProperty<int?>(_ => _.UserID);
		public int? UserID
		{
			get { return this.ReadProperty(User.UserIDProperty); }
			private set { this.LoadProperty(User.UserIDProperty, value); }
		}

		public static readonly PropertyInfo<string> UserNameProperty =
		  User.RegisterProperty<string>(_ => _.UserName);
		[Required(ErrorMessage = "The user name is required.")]
		public string UserName
		{
			get { return this.GetProperty(User.UserNameProperty); }
			set { this.SetProperty(User.UserNameProperty, value); }
		}

		public static readonly PropertyInfo<int?> UserRoleIDProperty =
		  User.RegisterProperty<int?>(_ => _.UserRoleID);
		public int? UserRoleID
		{
			get { return this.GetProperty(User.UserRoleIDProperty); }
			set { this.SetProperty(User.UserRoleIDProperty, value); }
		}
	}
}
