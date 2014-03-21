using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyVote.Core.Extensions;
using MyVote.Repository;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class CategoryTests
	{
		[TestMethod]
		public void Fetch()
		{
			var entity = EntityCreator.Create<MVCategory>();

			var category = DataPortal.FetchChild<Category>(entity);

			Assert.AreEqual(entity.CategoryID, category.ID, category.GetPropertyName(_ => _.ID));
			Assert.AreEqual(entity.CategoryName, category.Name, category.GetPropertyName(_ => _.Name));
		}
	}
}
