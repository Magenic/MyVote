using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using OpenQA.Selenium;
using System;

namespace Models
{
    // Page object for the MS login page
    public class MSLogin
    {
        // The MS login page url
        private const string PageUrl = "https://login.live.com/";

        // MS username 'By' ID
        private static By username = By.CssSelector("INPUT[name = 'loginfmt'],#cred-userid-inputtext");

        // MS password 'By' ID
        private static By password = By.CssSelector("INPUT[name = 'passwd'],#cred-password-inputtext");

        // Login button 'By' ID
        private static By loginButton = By.CssSelector("INPUT[type = 'submit'],#submit-button");

        /// Selenium Web Driver
        private IWebDriver webDriver;

        public MSLogin(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        // Enter MS Credentials
        public void EnterCredentials(string login, string pw)
        {
            this.AssertPageLoaded();
            this.webDriver.WaitForVisibleElement(username).SendKeys(login);
            this.webDriver.WaitForVisibleElement(password).SendKeys(pw);
            this.webDriver.FindElement(loginButton).Click();
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


