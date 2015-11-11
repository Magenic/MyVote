
namespace MyVote.UI.Helpers
{
    public interface IAppSettings
    {
		void Add<T>(string key, T value);
		bool TryGetValue<T>(string key, out T value);
		bool Remove(string key);
    }
}
