using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace MyVote.UI.Helpers
{
	public sealed class AppSettings : IAppSettings
	{
		public void Add(string key, string value)
		{
			ApplicationData.Current.RoamingSettings.Values[key] = value;
		}

		public bool TryGetValue(string key, out string value)
		{
			var result = false;
			if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
			{
				value = (string)ApplicationData.Current.RoamingSettings.Values[key];
				result = true;
			}
			else
			{
				value = string.Empty;
			}

			return result;
		}

		public bool Remove(string key)
		{
			return Windows.Storage.ApplicationData.Current.RoamingSettings.Values.Remove(key);
		}
	}
}
