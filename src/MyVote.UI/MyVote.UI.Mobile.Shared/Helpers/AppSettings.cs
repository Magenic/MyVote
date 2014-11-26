
namespace MyVote.UI.Helpers
{
    public class AppSettings : IAppSettings
    {
        public void Add(string key, object value)
        {
            var settings = new Refractored.Xam.Settings.Settings();
            settings.AddOrUpdateValue(key, value);
            settings.Save();
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            var settings = new Refractored.Xam.Settings.Settings();
            value = settings.GetValueOrDefault<T>(key);
            return value != null && !string.IsNullOrEmpty(value.ToString());
        }

        public bool Remove(string key)
        {
            object value;

            var found = this.TryGetValue<object>(key, out value);
            if (!found)
                return false;
            var settings = new Refractored.Xam.Settings.Settings();
            settings.AddOrUpdateValue(key, null);
            settings.Save();
            return true;
        }
    }
}
