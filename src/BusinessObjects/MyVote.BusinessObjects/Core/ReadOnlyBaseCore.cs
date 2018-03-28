﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Csla;
using MyVote.BusinessObjects.Core.Contracts;

#if !MOBILE
using MyVote.BusinessObjects.Attributes;
using MyVote.Data.Entities;
#endif

namespace MyVote.BusinessObjects.Core
{
	[System.Serializable]
	internal abstract class ReadOnlyBaseCore<T>
		: ReadOnlyBase<T>, IReadOnlyBaseCore
		where T : ReadOnlyBaseCore<T>
	{
		protected ReadOnlyBaseCore()
			: base()
		{ }

		protected virtual void Child_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		protected virtual void Child_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		protected virtual void DataPortal_Create()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

		protected virtual void DataPortal_Fetch()
		{
			throw new NotSupportedException(CoreMessages.ErrorMustOverrideDataPortalMethod);
		}

#if !MOBILE
		protected virtual List<string> IgnoredProperties
		{
			get
			{
				return new List<string>(new[] { nameof(this.Entities) });
			}
		}

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