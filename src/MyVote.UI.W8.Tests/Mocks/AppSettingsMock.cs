using MyVote.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class AppSettingsMock : IAppSettings
	{
		public Action<string, object> AddDelegate { get; set; }
		public void Add(string key, object value)
		{
			if (AddDelegate != null)
			{
				AddDelegate(key, value);
			}
		}

		public Func<string, object, bool> TryGetValueDelegate { get; set; }
		public bool TryGetValue<T>(string key, out T value)
		{
			value = default(T);
			if (TryGetValueDelegate != null)
			{
				return TryGetValueDelegate(key, value);
			}
			else
			{
				return false;
			}
		}

		public Func<string, bool> RemoveDelegate { get; set; }
		public bool Remove(string key)
		{
			if (RemoveDelegate != null)
			{
				return RemoveDelegate(key);
			}
			else
			{
				return false;
			}
		}
	}
}
