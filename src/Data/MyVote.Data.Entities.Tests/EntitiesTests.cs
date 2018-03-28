using Microsoft.Extensions.Configuration;
using FluentAssertions;
using System.Linq;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace MyVote.Data.Entities.Tests
{
	public sealed class EntitiesTests
	{
		[Fact(Skip = "needs to be refactored to use to use Environment.GetEnvironmentVariable(\"SQLCONNSTR_Entities\"")]
		public void GetPolls()
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json");
			var root = configuration.Build();

			using (var context = new EntitiesContext(root))
			{
				var polls = context.Mvpoll.ToList();
				polls.Count.Should().BeGreaterThan(0);

				var entity = (from u in context.Mvuser.Include(_ => _.UserRole)
								  select new { User = u, UserRole = u.UserRole }).ToList();
			}
		}
	}
}
