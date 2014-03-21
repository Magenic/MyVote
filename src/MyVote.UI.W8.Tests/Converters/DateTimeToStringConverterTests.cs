using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.UI.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Converters
{
	[TestClass]
	public sealed class DateTimeToStringConverterTests
	{
		[TestMethod]
		public void ConvertDate()
		{
			// Arrange
			var converter = new DateTimeToStringConverter();

			// Act
			var result = converter.Convert(DateTime.Parse("01/01/2013"), null, null, null).ToString();

			// Assert
			Assert.AreEqual("1/1/2013", result);
		}

		[TestMethod]
		public void ConvertNonDate()
		{
			// Arrange
			var converter = new DateTimeToStringConverter();

			// Act
			var result = converter.Convert(new object(), null, null, null).ToString();

			// Assert
			Assert.AreEqual(string.Empty, result);
		}

		[TestMethod, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BackDate")]
		public void ConvertBackDate()
		{
			// Arrange
			var converter = new DateTimeToStringConverter();

			// Act
			var result = (DateTime)converter.ConvertBack("01/01/2013", null, null, null);

			// Assert
			Assert.AreEqual(DateTime.Parse("1/1/2013"), result);
		}

		[TestMethod]
		public void ConvertBackNonDate()
		{
			// Arrange
			var converter = new DateTimeToStringConverter();

			// Act
			var result = (DateTime)converter.ConvertBack(Guid.NewGuid().ToString(), null, null, null);

			// Assert
			Assert.AreEqual(new DateTime(), result);
		}

		[TestMethod]
		public void ConvertBackNonDateNullable()
		{
			// Arrange
			var converter = new DateTimeToStringConverter { IsDateTimeNullable = true };

			// Act
			var result = converter.ConvertBack(Guid.NewGuid().ToString(), null, null, null);

			// Assert
			Assert.IsNull(result);
		}
	}
}
