using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using OpenQA.Selenium;
using System;

namespace Models
{
    // Page object for the Google login page
    public class GoogleLogin
    {
        // The Google login page url
        private const string PageUrl = "https://accounts.google.com/";

        // Google email 'By' ID
        private static By username = By.Id("Email");

        // Next button 'By' ID
        private static By nextButton = By.Id("next");

        // Google password 'By' ID
        private static By password = By.Id("Passwd");

        // Login button 'By' ID
        private static By loginButton = By.Id("signIn");

        /// Selenium Web Driver
        private IWebDriver webDriver;

        public GoogleLogin(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        // Enter Google Credentials
        public void EnterCredentials(string login, string pw)
        {
            this.AssertPageLoaded();
            this.webDriver.WaitForVisibleElement(username).SendKeys(login);
            this.webDriver.FindElement(nextButton).Click();
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
