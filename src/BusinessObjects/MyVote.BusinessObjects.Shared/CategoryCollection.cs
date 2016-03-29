using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using System;

#if !NETFX_CORE && !MOBILE
using MyVote.BusinessObjects.Attributes;
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
	[Serializable]
	internal sealed class CategoryCollection
		: ReadOnlyListBaseCore<CategoryCollection, ICategory>, ICategoryCollection
	{
#if !NETFX_CORE && !MOBILE
		protected override void DataPortal_Fetch()
		{
			this.IsReadOnly = false;

			try
			{
				foreach (var category in (from c in this.Entities.MVCategories
												  select c).ToList())
				{
					this.Add(this.categoryFactory.FetchChild(category));
				}
			}
			finally
			{
				this.IsReadOnly = true;
			}
		}

		[NonSerialized]
		private IEntities entities;
		[Dependency]
		public IEntities Entities
		{
			get { return this.entities; }
			set { this.entities = value; }
		}

		[NonSerialized]
		private IObjectFactory<ICategory> categoryFactory;
		[Dependency]
		public IObjectFactory<ICategory> CategoryFactory
		{
			get { return this.categoryFactory; }
			set { this.categoryFactory = value; }
		}
#endif
	}
}
