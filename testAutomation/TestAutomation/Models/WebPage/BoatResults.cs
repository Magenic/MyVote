using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;


namespace Models
{

    public class BoatResults : MyVoteBasePageModel
    {
        // The page URL
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/pollResult/6";

        #region Accessors

        // Vote Results
        private static By voteResults = By.XPath(".//*[@id='highcharts-10']/svg/rect");

        #endregion

        #region Methods

        public BoatResults(IWebDriver webDriver) : base(webDriver)
        {
        }

        // Verify we are on the correct page
        public override void AssertPageLoaded()
        {
            this.webDriver.WaitForVisibleElement(voteResults);
            Assert.IsTrue(
                this.webDriver.Url.Equals(pageUrl, System.StringComparison.CurrentCultureIgnoreCase),
                "Expected to be on '{0}', but was on '{1}' instead",
                pageUrl,
                this.webDriver.Url);
        }

        #endregion
    }
}
