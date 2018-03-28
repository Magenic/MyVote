﻿using Csla;
using MyVote.BusinessObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.BusinessObjects
{
	 [System.Serializable]
	public sealed class PollResultsCriteria
		: CriteriaBaseCore<PollResultsCriteria>
	{
		 public PollResultsCriteria()
		 {
		 }

		 public PollResultsCriteria(int? userId, int pollId)
			: base()
		{
			this.UserID = userId;
			this.PollID = pollId;
		}

		public static readonly PropertyInfo<int> PollIDProperty =
			PollResultsCriteria.RegisterProperty<int>(_ => _.PollID);
		public int PollID
		{
			get { return this.ReadProperty(PollResultsCriteria.PollIDProperty); }
			private set { this.LoadProperty(PollResultsCriteria.PollIDProperty, value); }
		}

		public static readonly PropertyInfo<int?> UserIDProperty =
			PollResultsCriteria.RegisterProperty<int?>(_ => _.UserID);
		public int? UserID
		{
			get { return this.ReadProperty(PollResultsCriteria.UserIDProperty); }
			private set { this.LoadProperty(PollResultsCriteria.UserIDProperty, value); }
		}
	}
}
