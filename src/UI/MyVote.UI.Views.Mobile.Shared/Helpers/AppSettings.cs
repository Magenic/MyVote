using Plugin.Settings;
using System;

namespace MyVote.UI.Helpers
{
	public sealed class AppSettings : IAppSettings
	{
		public void Add<T>(string key, T value)
		{
			var settings = CrossSettings.Current;
            settings.AddOrUpdateValue(key, value);
		}

		public bool TryGetValue<T>(string key, out T value)
		{
			var settings = CrossSettings.Current;
		    try
		    {
		        value = settings.GetValueOrDefault<T>(key);
		    }
		    catch (NullReferenceException)
		    {
		        value = default(T);
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