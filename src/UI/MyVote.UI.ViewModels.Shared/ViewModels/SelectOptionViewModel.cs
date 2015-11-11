
namespace MyVote.UI.ViewModels
{
    public sealed class SelectOptionViewModel<T>
    {
		public string Display { get; set; }
		public T Value { get; set; }
    }
}
