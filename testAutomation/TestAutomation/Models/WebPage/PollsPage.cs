using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Models
{

    public class PollsPage : MyVoteBasePageModel
    {
        // The page URL
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/polls";

        #region Accessors

        // Filter Dropdown field
        private static By filter = By.Id("filter");

        // Add Poll button
        private static By addPollBtn = By.CssSelector("button[ng-click='addPoll()']");

        // Logout button
        private static By logoutButton = By.ClassName("logout-link");

        // Signed in as
        private static By signedInAs = By.ClassName("ng-binding");

        private static By responseCount = By.CssSelector("h4");

        private static By firstPoll = By.CssSelector(".poll-group.ng-scope:nth-of-type(1)>a:nth-of-type(1)");
        private static By viewLabel = By.Id(".poll-actions.ng-pristine.ng-valid>label");


        // Best Wakeboard boat poll
        private static By boatPoll = By.XPath(".//*[@id='ng-app']/body/div[1]/div/div[1]/a[1]");

        // Cups of Water poll
        private static By waterPoll = By.XPath(".//*[@id='ng-app']/body/div[1]/div/div[2]/a[1]");

        #endregion

        #region Methods

        public  PollsPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        // Verify we are on the correct page
        public override void AssertPageLoaded()
        {
            this.webDriver.WaitForClickableElement(filter);
            Assert.IsTrue(
                this.webDriver.Url.Equals(pageUrl, System.StringComparison.CurrentCultureIgnoreCase),
                "Expected to be on '{0}', but was on '{1}' instead",
                pageUrl,
                this.webDriver.Url);
        }

        /// <summary>
        /// This will sort the polls by whatever value is passed 
        /// </summary>
        /// <param name="sortType"></param>
        public void SortPollsBy(string sortType)
        {
            
            var selectElement = new SelectElement(this.webDriver.WaitForClickableElement(filter));
            selectElement.SelectByText(sortType);
            this.webDriver.WaitForPageLoad();
        }

        // Add New Poll
        public AddPollPage StartNewPoll()
        {

            this.webDriver.WaitForClickableElement(addPollBtn).Click();
            return new AddPollPage(this.webDriver);
        }

        // Add New Poll
        public ViewPollPage OpenFirstPoll()
        {

            this.webDriver.WaitForClickableElement(firstPoll).Click();
            return new ViewPollPage(this.webDriver);
        }

        // Select the Boat Poll
        public BoatPoll openBoatPoll()
        {
            this.webDriver.WaitForVisibleElement(boatPoll).Click();
            return new BoatPoll(this.webDriver);
        }


        #endregion
    }
}