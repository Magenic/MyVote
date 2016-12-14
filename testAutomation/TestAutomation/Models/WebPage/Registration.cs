using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;


namespace Models
{

    public class RegistrationPage : MyVoteBasePageModel
    {
        // The Default page URL
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/registration";

        #region Accessors

        // Email Address field
        private static By email = By.Id("email");

        // MyVote screen name field
        private static By screenName = By.Id("screenName");

        // DOB field
        private static By DOB = By.Id("birthDate");

        // Sex field
        private static By sex = By.Id("sex");

        // ZIP Code field
        private static By zip = By.Id("zip");

        // Let's Get Started button
        private static By regButton = By.CssSelector("BUTTON[type='submit']");
        #endregion

        #region Methods

        // Initializes a new instance of the RegistrationPage
        public RegistrationPage(IWebDriver webDriver) : base(webDriver)
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

        // Enter Registration details on page
        public PollsPage EnterRegInfo(string emailR, string screenNameR, string DOBR, string sexR, string zipR)
        {
            this.webDriver.WaitForVisibleElement(email).SendKeys(emailR);
            this.webDriver.WaitForVisibleElement(screenName).SendKeys(screenNameR);
            this.webDriver.WaitForVisibleElement(DOB).SendKeys(DOBR);
            this.webDriver.WaitForVisibleElement(sex).SendKeys(sexR);
            this.webDriver.WaitForVisibleElement(zip).SendKeys(zipR);
            this.webDriver.WaitForVisibleElement(regButton).Click();

            return new PollsPage(this.webDriver);
        }

        #endregion
    }
}
