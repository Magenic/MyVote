using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using OpenQA.Selenium;
using System;

namespace Models
{
    // Page object for the Twitter login page
    public class TwitterLogin
    {
        // The Twitter login page url
        private const string PageUrl = "twitter";

        // Twitter username or email element 'By' ID
        private static By username = By.Id("username_or_email");

        // Twitter password 'By' ID
        private static By password = By.Id("password");

        // Authorization button 'By' ID
        private static By logicButton = By.Id("allow");

        /// Selenium Web Driver
        private IWebDriver webDriver;

        public TwitterLogin(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        // Enter Twitter Credentials
        public void EnterCredentials(string login, string pw)
        {
            this.webDriver.WaitForClickableElement(username).SendKeys(login);
            this.webDriver.WaitForClickableElement(password).SendKeys(pw);
            this.webDriver.WaitForClickableElement(logicButton).Click();
        }

        // Verify we are on the login page
        public void AssertPageLoaded()
        {
            //Assert depends on what testing framework is being used
            Assert.IsTrue(
            this.webDriver.WaitUntilVisibleElement(username),
                "The web page '{0}' is not loaded since I can find all the elements on the page",
                PageUrl);
        }

    }
}
