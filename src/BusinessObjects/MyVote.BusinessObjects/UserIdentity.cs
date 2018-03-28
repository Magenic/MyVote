using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using System;

#if !MOBILE
using Microsoft.EntityFrameworkCore;
using MyVote.BusinessObjects.Attributes;
using MyVote.Data.Entities;
using Csla.Core;
#endif

namespace MyVote.BusinessObjects
{
	[Serializable]
	internal sealed class UserIdentity
		: CslaIdentityCore<UserIdentity>, IUserIdentity
	{
#if !MOBILE
		private void DataPortal_Fetch(string profileId)
		{
			var entity = (from u in this.Entities.Mvuser
							  where u.ProfileId == profileId
							  select new { User = u, UserRole = u.UserRole }).SingleOrDefault();
			this.IsAuthenticated = entity != null;

			if (this.IsAuthenticated)
			{
				this.ProfileID = profileId;
				this.UserID = entity.User.UserId;
				this.UserName = entity.User.UserName;

				if (entity.UserRole != null)
				{
					this.Roles = new MobileList<string>();
					this.Roles.Add(entity.UserRole.UserRoleName);
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

#if !MOBILE
		[NonSerialized]
		private IEntitiesContext entities;
		[Dependency]
		public IEntitiesContext Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}
#endif
	}
}
