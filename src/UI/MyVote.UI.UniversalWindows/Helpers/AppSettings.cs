using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace MyVote.UI.Helpers
{
	public sealed class AppSettings : IAppSettings
	{
		public void Add<T>(string key, T value)
		{
			ApplicationData.Current.RoamingSettings.Values[key] = value;
		}

		public bool TryGetValue<T>(string key, out T value)
		{
			var result = false;
			if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
			{
				value = (T)ApplicationData.Current.RoamingSettings.Values[key];
				result = true;
			}
			else
			{
				value = default(T);
			}

			return result;
		}

		public bool Remove(string key)
		{
			return Windows.Storage.ApplicationData.Current.RoamingSettings.Values.Remove(key);
		}
	}
}
