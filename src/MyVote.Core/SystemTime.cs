using System;

namespace MyVote.Core
{
	/// <summary>
	/// Provides a mechanism to retrieve the current <c>DateTime</c>
	/// in UTC as well as allow developers to mock the implementation via a
	/// new function.
	/// </summary>
	public static class SystemTime
	{
		/// <summary>
		/// Gets the current <c>DateTime</c> value in UTC.
		/// </summary>
		public static Func<DateTime> UtcNow = () => DateTime.UtcNow;
	}
}
