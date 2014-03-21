using System;
using Csla;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.BusinessObjects.Contracts
{
	public interface IUser
		: IBusinessBaseCore
	{
		DateTime? BirthDate { get; set; }
		string EmailAddress { get; set; }
		string FirstName { get; set; }
		string Gender { get; set; }
		string LastName { get; set; }
		string PostalCode { get; set; }
		string ProfileID { get; }
		int? UserID { get; }
		string UserName { get; set; }
		int? UserRoleID { get; set; }
	}
}
