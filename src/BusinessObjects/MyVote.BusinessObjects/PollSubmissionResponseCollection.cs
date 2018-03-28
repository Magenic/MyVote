using System;
using System.Diagnostics.CodeAnalysis;
using Csla;
using Csla.Serialization.Mobile;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.BusinessObjects.Attributes;

namespace MyVote.BusinessObjects
{
	[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
	[Serializable]
	internal sealed class PollSubmissionResponseCollection
		: BusinessListBaseCore<PollSubmissionResponseCollection, IPollSubmissionResponse>, IPollSubmissionResponseCollection
	{
		protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
		{
			this.isLoading = true;
			base.OnSetChildren(info, formatter);
			this.isLoading = false;
		}

#if !MOBILE
		protected override void Child_Create() { }

		private void Child_Create(BusinessList<IPollOption> options)
		{
			this.isLoading = true;

			try
			{
				foreach (var option in options)
				{
					this.Add(this.pollSubmissionResponseFactory.CreateChild(option));
				}
			}
			finally
			{
				this.isLoading = false;
			}
		}

		[NonSerialized]
		private IObjectFactory<IPollSubmissionResponse> pollSubmissionResponseFactory;
		[Dependency]
		public IObjectFactory<IPollSubmissionResponse> PollSubmissionResponseFactory
		{
			get { return this.pollSubmissionResponseFactory; }
			set { this.pollSubmissionResponseFactory = value; }
		}
#endif

		protected override IPollSubmissionResponse AddNewCore()
		{
			throw new NotSupportedException("Items cannot be added to the collection.");
		}

		protected override void ClearItems()
		{
			throw new NotSupportedException("Items cannot be cleared from the collection.");
		}

		protected override void InsertItem(int index, IPollSubmissionResponse item)
		{
			if (!this.isLoading)
			{
				throw new NotSupportedException("Items cannot be inserted into the collection.");
			}
			else
			{
				base.InsertItem(index, item);
			}
		}

		protected override void MoveItem(int oldIndex, int newIndex)
		{
			throw new NotSupportedException("Items cannot be moved in the collection.");
		}

		protected override void RemoveItem(int index)
		{
			throw new NotSupportedException("Items cannot be removed from the collection.");
		}

#pragma warning disable 649
		private bool isLoading;
#pragma warning restore 649
	}
}
