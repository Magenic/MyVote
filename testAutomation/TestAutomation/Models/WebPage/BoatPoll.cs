using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;
using System;


namespace Models
{

    public class BoatPoll : MyVoteBasePageModel
    {
        // The page URL
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/viewPoll/6";

        #region Accessors

        // Option 1
        private static By option1 = By.Id("option21");

        // Option 2
        private static By option2 = By.Id("option22");

        // Option 3
        private static By option3 = By.Id("option23");

        // Option 4
        private static By option4 = By.Id("option24");

        // Option 5
        private static By option5 = By.Id("option25");

        // Submit Button
        private static By submitButton = By.CssSelector("button[ng-click='submit()']");

        // View Results Button
        private static By resultsButton = By.XPath(".//*[@id='ng-app']/body/div[1]/section/div[1]/a[1]");

        // Back to Polls Button
        private static By backButton = By.XPath(".//*[@id='ng-app']/body/div[1]/section/div[1]/a[2]");

        #endregion

        #region Methods

        public BoatPoll(IWebDriver webDriver) : base(webDriver)
        {

        }

        // Verify we are on the correct page
        public override void AssertPageLoaded()
        {
            Assert.IsTrue(
                this.webDriver.Url.Equals(pageUrl, System.StringComparison.CurrentCultureIgnoreCase),
                "Expected to be on '{0}', but was on '{1}' instead",
                pageUrl,
                this.webDriver.Url);
        }

        // Vote and submit poll
        public void VoteBoat()
        {
            this.webDriver.WaitForVisibleElement(Selection()).Click();
            this.webDriver.WaitForVisibleElement(submitButton).Click();
        }

        // Choose a random option to vote for
        public By Selection()
        {
            By[] votes = { option1, option2, option3, option4, option5 };
            Random r = new Random();
            int index = r.Next(0, votes.Length);
            By choice = votes[index];
            return choice;
        }

        #endregion
    }
}
