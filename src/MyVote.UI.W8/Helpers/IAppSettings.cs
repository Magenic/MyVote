
namespace MyVote.UI.Helpers
{
	public interface IAppSettings
	{
		void Add(string key, object value);
		bool TryGetValue<T>(string key, out T value);
		bool Remove(string key);
	}
}
