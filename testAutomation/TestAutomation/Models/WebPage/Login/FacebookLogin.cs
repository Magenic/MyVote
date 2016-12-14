using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using OpenQA.Selenium;
using System;

namespace Models
{
    // Page object for the Facebook login page
    public class FacebookLogin
    {
        // The Twitter login page url
        private const string PageUrl = "https://www.facebook.com/login.php?";

        // Facebook username 'By' ID
        private static By username = By.Id("email");

        // Facebook password 'By' ID
        private static By password = By.Id("pass");

        // Login button 'By' ID
        private static By loginButton = By.Id("loginbutton");

        /// Selenium Web Driver
        private IWebDriver webDriver;

        public FacebookLogin(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        // Enter Facebook Credentials
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
