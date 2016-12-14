using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Magenic.MaqsFramework.Utilities.ResponseTime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// Page object for the landing page
    /// </summary>
    public class LandingPage : MyVoteBasePageModel
    {
        /// <summary>
        /// The page URL
        /// </summary>
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/landing";

        #region Accessors

        // Twitter SSO button
        private static By twitterSignIn = By.CssSelector("img[ng-src='/Content/twitter.png']");
        
        // FaceBook SSO button
        private static By facebookSignIn = By.CssSelector("img[ng-src='/Content/fb.png']");

        // Twitter SSO button
        private static By microsoftSignIn= By.CssSelector("img[ng-src='/Content/ms.png']");

        // Twitter SSO button
        private static By googleSignIn = By.CssSelector("img[ng-src='/Content/google.png']");

        // Error message
        private static By errorMessage = By.CssSelector(".topbar.topbar-error");

        // Landing Menu table
        private static By menu = By.ClassName("landing-hmenu");
        #endregion

        #region Methods
        // Selenium Web Driver
       // private IWebDriver webDriver;

        /// <summary>
        /// Initializes a new instance of the LandingPage
        /// </summary>
        /// <param name="webDriver"></param>
        public LandingPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        /// <summary>
        /// Open the MyVote Landing page
        /// </summary>
        public void OpenPage()
        {
            this.webDriver.Navigate().GoToUrl(pageUrl);
            this.AssertPageLoaded();
            this.webDriver.WaitForVisibleElement(menu);
        }

        /// <summary>
        /// Verify we are on the correct page
        /// </summary>
        public override void AssertPageLoaded()
        {
            Assert.IsTrue(
                this.webDriver.Url.Equals(pageUrl, System.StringComparison.CurrentCultureIgnoreCase),
                "Expected to be on '{0}', but was on '{1}' instead",
                pageUrl,
                this.webDriver.Url);
        }

        /// <summary>
        /// Click to sign in through Twitter
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public RegistrationPage SignInNewTwitter(string username, string password, ResponseTimes response = null)
        {
            SignInTwitter(username, password, response);
   
            return new RegistrationPage(this.webDriver);
        }

        /// <summary>
        /// Click to sign in through Twitter
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public PollsPage SignInExistingTwitter(string username, string password, ResponseTimes response = null)
        {
            SignInTwitter(username, password, response);

            return new PollsPage(this.webDriver);
        }

        /// <summary>
        /// Click to sign in through Twitter
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private void SignInTwitter(string username, string password, ResponseTimes response = null)
        {
            //Open the twitter Login
            this.webDriver.FindElement(twitterSignIn).Click();

            // Get the current Window so you can switch back later 
            string myVoteWindow = this.webDriver.CurrentWindowHandle;

            // Method To Find and Switch to new window
            SwitchWindow(myVoteWindow);

            //Login to twitter
            TwitterLogin TP = new TwitterLogin(this.webDriver);
            TP.AssertPageLoaded();

            if (response != null)
            {
                response.StartTimer("Twitter Login");
            }

            TP.EnterCredentials(username, password);

            //Switch back to the main window
            this.webDriver.SwitchTo().Window(myVoteWindow);
        }

        /// <summary>
        /// Click to sign in through Facebook
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public PollsPage SignInFacebook(string username, string password, ResponseTimes response = null)
        {
            //Open the Facebook Login
            this.webDriver.FindElement(facebookSignIn).Click();

            // Get the current Window so you can switch back later 
            string myVoteWindow = this.webDriver.CurrentWindowHandle;

            // Method To Find and Switch to new window
            SwitchWindow(myVoteWindow);

            //Login to Facebook
            FacebookLogin TP = new FacebookLogin(this.webDriver);
            TP.AssertPageLoaded();

            if (response != null)
            {
                response.StartTimer("Facebook Login");
            }

            TP.EnterCredentials(username, password);

            //Switch back to the main window
            this.webDriver.SwitchTo().Window(myVoteWindow);

            return new PollsPage(this.webDriver);
        }

        /// <summary>
        /// Click to sign in through Microsoft
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public PollsPage SignInMicrosoft(string username, string password, ResponseTimes response = null)
        {
            //Open the Microsoft Login
            this.webDriver.FindElement(microsoftSignIn).Click();

            // Get the current Window so you can switch back later 
            string myVoteWindow = this.webDriver.CurrentWindowHandle;

            // Method To Find and Switch to new window
            SwitchWindow(myVoteWindow);

            //Login to Microsoft
            MSLogin microsoft = new MSLogin(this.webDriver);
            microsoft.AssertPageLoaded();

            if (response != null)
            {
                response.StartTimer("Microsoft Login");
            }

            microsoft.EnterCredentials(username, password);

            //Switch back to the main window
            this.webDriver.SwitchTo().Window(myVoteWindow);

            return new PollsPage(this.webDriver);
        }

        /// <summary>
        /// Click to sign in through Google+
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public PollsPage SignInGoogle(string username, string password, ResponseTimes response = null)
        {
            //Open the Google+ Login
            this.webDriver.FindElement(googleSignIn).Click();

            // Get the current Window so you can switch back later 
            string myVoteWindow = this.webDriver.CurrentWindowHandle;

            // Method To Find and Switch to new window
            SwitchWindow(myVoteWindow);

            //Login to Google+
            GoogleLogin google = new GoogleLogin(this.webDriver);

            google.AssertPageLoaded();

            if (response != null)
            {
                response.StartTimer("Google Login");
            }

            google.EnterCredentials(username, password);

            //Switch back to the main window
            this.webDriver.SwitchTo().Window(myVoteWindow);

            return new PollsPage(this.webDriver);
        }

        /// <summary>
        /// Find and switch to the new window
        /// </summary>
        /// <param name="MyVoteWindow"></param>
        public void SwitchWindow(string MyVoteWindow)
        {
            // Get new window
            IList<string> windowHandles = this.webDriver.WindowHandles;
            string ssoWindow = string.Empty;
            foreach (string w in windowHandles)
            {
                if (MyVoteWindow != w)
                {
                    ssoWindow = w;
                }
            }

            // Switch to new window
            this.webDriver.SwitchTo().Window(ssoWindow);
        }

        /// <summary>
        /// Close the sign in window without signing in 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SignInBadCredentials(string username, string password, ResponseTimes response = null)
        {
            //Open the Facebook Login
            this.webDriver.FindElement(facebookSignIn).Click();

            // Get the current Window so you can switch back later 
            string myVoteWindow = this.webDriver.CurrentWindowHandle;

            // Method To Find and Switch to new window
            SwitchWindow(myVoteWindow);
            this.webDriver.Close();

            //Switch back to the main window
            this.webDriver.SwitchTo().Window(myVoteWindow);
        }

        /// <summary>
        /// Find if the error message is showing
        /// </summary>
        public bool AssertFailShows()
        {
            //try catch to see if the value banner is showing 
            try
            {
                this.webDriver.FindElement(errorMessage).Click();
                return true;
            }
            catch
            {
                return false; 
            }
        }

            #endregion
        }
    }
