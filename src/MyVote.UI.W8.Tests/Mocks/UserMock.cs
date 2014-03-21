using MyVote.BusinessObjects.Contracts;
using MyVote.UI.W8.Tests.Mocks.Base;
using System;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class UserMock : BusinessBaseCoreMock, IUser
	{
		public DateTime? BirthDate { get; set; }

		public string EmailAddress { get; set; }

		public string FirstName { get; set; }

		public string Gender { get; set; }

		public string LastName { get; set; }

		public string PostalCode { get; set; }

		public string ProfileID { get; set; }

		public int? UserID { get; set; }

		public string UserName { get; set; }

		public int? UserRoleID { get; set; }

	}
}
