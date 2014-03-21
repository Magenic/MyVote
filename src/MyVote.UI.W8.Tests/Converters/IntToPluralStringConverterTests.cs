using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.UI.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyVote.UI.W8.Tests.Converters
{
	[TestClass]
	public sealed class IntToPluralStringConverterTests
	{
		[TestMethod]
		public void ConvertWith0()
		{
			// Arrange
			var converter = new IntToPluralStringConverter();

			// Act
			var result = converter.Convert(0, null, "Response", null);

			// Assert
			Assert.AreEqual("Responses", result.ToString());
		}

		[TestMethod]
		public void ConvertWith1()
		{
			// Arrange
			var converter = new IntToPluralStringConverter();

			// Act
			var result = converter.Convert(1, null, "Response", null);

			// Assert
			Assert.AreEqual("Response", result.ToString());
		}

		[TestMethod]
		public void ConvertWith2()
		{
			// Arrange
			var converter = new IntToPluralStringConverter();

			// Act
			var result = converter.Convert(2, null, "Response", null);

			// Assert
			Assert.AreEqual("Responses", result.ToString());
		}

		[TestMethod]
		public void ConvertWithGreaterThan2()
		{
			// Arrange
			var converter = new IntToPluralStringConverter();

			// Act
			var result = converter.Convert(new Random().Next(3, int.MaxValue), null, "Response", null);

			// Assert
			Assert.AreEqual("Responses", result.ToString());
		}

		[TestMethod]
		public void ConvertWithNullParameter()
		{
			// Arrange
			var converter = new IntToPluralStringConverter();

			// Act
			var result = converter.Convert(new Random().Next(3, int.MaxValue), null, null, null);

			// Assert
			Assert.AreEqual(string.Empty, result.ToString());
		}
	}
}
