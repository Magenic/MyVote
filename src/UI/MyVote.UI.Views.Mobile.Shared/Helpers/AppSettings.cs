using Plugin.Settings;
using System;

namespace MyVote.UI.Helpers
{
	public sealed class AppSettings : IAppSettings
	{
		public void Add(string key, string value)
		{
			var settings = CrossSettings.Current;
            settings.AddOrUpdateValue(key, value);
		}

		public bool TryGetValue(string key, out string value)
		{
			var settings = CrossSettings.Current;
		    try
		    {
		        value = settings.GetValueOrDefault(key, string.Empty);
		    }
		    catch (NullReferenceException)
		    {
		        value = string.Empty;
		        return false;
		    }
		    return value != null && !string.IsNullOrEmpty(value.ToString());
		}

		public bool Remove(string key)
		{
            var settings = CrossSettings.Current;
            settings.Remove(key);
            return true;
		}
	}
}