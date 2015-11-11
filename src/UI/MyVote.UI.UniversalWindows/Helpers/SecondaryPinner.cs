using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MyVote.UI.Helpers
{
	public sealed class SecondaryPinner : ISecondaryPinner
	{
		private const string TileIdFormat = "MyVote.Secondary.{0}";

		public async Task<bool> PinPoll(FrameworkElement anchorElement, int pollId, string question)
		{
			var isPinned = false;
			Uri logoUri = new Uri("ms-appx:///Assets/Logo.png");

			var tileId = string.Format(SecondaryPinner.TileIdFormat, pollId);
			if (!SecondaryTile.Exists(tileId))
			{
				var secondaryTile = new SecondaryTile(
					tileId,
					question,
					pollId.ToString(),
					logoUri,
					TileSize.Default);

				isPinned = await secondaryTile.RequestCreateForSelectionAsync(
						GetElementRect(anchorElement), Placement.Above);
			}

			return isPinned;
		}

		public async Task<bool> UnpinPoll(FrameworkElement anchorElement, int pollId)
		{
			var wasUnpinned = false;

			var tileId = string.Format(SecondaryPinner.TileIdFormat, pollId);
			if (SecondaryTile.Exists(tileId))
			{
				var secondaryTile = new SecondaryTile(tileId);
				wasUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(
					GetElementRect(anchorElement), Placement.Above);
			}

			return wasUnpinned;
		}

		public bool IsPollPinned(int pollId)
		{
			return SecondaryTile.Exists(string.Format(SecondaryPinner.TileIdFormat, pollId));
		}

		private static Rect GetElementRect(FrameworkElement element)
		{
			GeneralTransform buttonTransform = element.TransformToVisual(null);
			Point point = buttonTransform.TransformPoint(new Point());
			return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
		}
	}
}
