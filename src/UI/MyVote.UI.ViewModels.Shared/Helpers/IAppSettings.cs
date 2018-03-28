
namespace MyVote.UI.Helpers
{
    public interface IAppSettings
    {
		void Add(string key, string value);
		bool TryGetValue(string key, out string value);
		bool Remove(string key);
    }
}
