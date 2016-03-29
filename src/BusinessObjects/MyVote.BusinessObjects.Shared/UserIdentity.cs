using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using System;

#if !NETFX_CORE && !MOBILE
using MyVote.BusinessObjects.Attributes;
using MyVote.Data.Entities;
using System.Data.Entity;
using Csla.Core;
#endif

namespace MyVote.BusinessObjects
{
	[System.Serializable]
	internal sealed class UserIdentity
		: CslaIdentityCore<UserIdentity>, IUserIdentity
	{
#if !NETFX_CORE && !MOBILE
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

		public static readonly PropertyInfo<string> ProfileIDProperty =
			UserIdentity.RegisterProperty<string>(_ => _.ProfileID);
		public string ProfileID
		{
			get { return this.ReadProperty(UserIdentity.ProfileIDProperty); }
			private set { this.LoadProperty(UserIdentity.ProfileIDProperty, value); }
		}

		public static readonly PropertyInfo<int?> UserIDProperty =
			UserIdentity.RegisterProperty<int?>(_ => _.UserID);
		public int? UserID
		{
			get { return this.ReadProperty(UserIdentity.UserIDProperty); }
			private set { this.LoadProperty(UserIdentity.UserIDProperty, value); }
		}

		public static readonly PropertyInfo<string> UserNameProperty =
			UserIdentity.RegisterProperty<string>(_ => _.UserName);
		public string UserName
		{
			get { return this.ReadProperty(UserIdentity.UserNameProperty); }
			private set { this.LoadProperty(UserIdentity.UserNameProperty, value); }
		}

#if !NETFX_CORE && !MOBILE
		[NonSerialized]
		private IEntities entities;
		[Dependency]
		public IEntities Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}
#endif
	}
}
