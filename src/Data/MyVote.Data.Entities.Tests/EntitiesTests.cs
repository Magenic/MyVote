using FluentAssertions;
using System.Linq;
using Xunit;

namespace MyVote.Data.Entities.Tests
{
	public sealed class EntitiesTests
	{
		[Fact]
		public void GetPolls()
		{
			using (var context = Entities.GetContext())
			{
				var polls = context.MVPolls.ToList();
				polls.Count.Should().BeGreaterThan(0);
			}
		}
	}
}
