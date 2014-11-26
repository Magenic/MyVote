using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
using System.Data.Entity;
using Csla.Core;
#endif

namespace MyVote.BusinessObjects
{
#if (!NETFX_CORE && !WINDOWS_PHONE) || ANDROID || IOS
	[System.Serializable]
#else
	[Csla.Serialization.Serializable]
#endif
	internal sealed class UserIdentity
		: CslaIdentityScopeCore<UserIdentity>, IUserIdentity
	{
#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		private void DataPortal_Fetch(string profileId)
		{
			var entity = (from u in this.Entities.MVUsers.Include(_ => _.MVUserRole)
							  where u.ProfileID == profileId
							  select u).SingleOrDefault();
			this.IsAuthenticated = entity != null;

			if (this.IsAuthenticated)
			{
				this.ProfileID = profileId;
				this.UserID = entity.UserID;
				this.UserName = entity.UserName;

				if (entity.MVUserRole != null)
				{
					this.Roles = new MobileList<string>();
					this.Roles.Add(entity.MVUserRole.UserRoleName);
				}
			}
		}
#endif

		public static PropertyInfo<string> ProfileIDProperty =
			UserIdentity.RegisterProperty<string>(_ => _.ProfileID);
		public string ProfileID
		{
			get { return this.ReadProperty(UserIdentity.ProfileIDProperty); }
			private set { this.LoadProperty(UserIdentity.ProfileIDProperty, value); }
		}

		public static PropertyInfo<int?> UserIDProperty =
			UserIdentity.RegisterProperty<int?>(_ => _.UserID);
		public int? UserID
		{
			get { return this.ReadProperty(UserIdentity.UserIDProperty); }
			private set { this.LoadProperty(UserIdentity.UserIDProperty, value); }
		}

		public static PropertyInfo<string> UserNameProperty =
			UserIdentity.RegisterProperty<string>(_ => _.UserName);
		public string UserName
		{
			get { return this.ReadProperty(UserIdentity.UserNameProperty); }
			private set { this.LoadProperty(UserIdentity.UserNameProperty, value); }
		}
	}
}
