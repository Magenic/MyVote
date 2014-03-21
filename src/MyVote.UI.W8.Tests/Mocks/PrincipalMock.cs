using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class PrincipalMock : IPrincipal
	{
		public IIdentity Identity { get; set; }

		public Func<string, bool> IsInRoleDelegate { get; set; }
		public bool IsInRole(string role)
		{
			if (IsInRoleDelegate != null)
			{
				return IsInRoleDelegate(role);
			}
			else
			{
				return false;
			}
		}
	}
}
