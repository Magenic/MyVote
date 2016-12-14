using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using System;
using Models;
using System.Collections.Generic;
using Magenic.MaqsFramework.BaseDatabaseTest;

namespace Tests
{
    // Log into MyVote using each SSO
    [TestClass]
    public class LoginTest : BaseSeleniumTest
    {

        [ClassInitialize]
        public static void TestSetup(TestContext context)
        {
            // Do database setup
            using (DatabaseConnectionWrapper wrapper = new DatabaseConnectionWrapper(DatabaseConfig.GetConnectionString()))
            {
                int removed = wrapper.NonQueryAndGetRowsAffected("Delete from [dbo].[MVUser] where [ProfileID] = 'Twitter:768151213946785792'");
                Assert.IsTrue(removed <= 1, "Expected to remove 1 or 0");
            }
        }

        /// <summary>
        /// Login to MyVote using Twitter authentication 
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI), TestCategory(Categories.Login)]
        public void NewLogin()
        {
            // Open Landing Page
            LandingPage landing = new LandingPage(this.WebDriver);

            landing.OpenPage();

            //Sign in to twitter with the login credentials 
            RegistrationPage register = landing.SignInNewTwitter(Config.GetValue("OtherTwitterUsername"), Config.GetValue("TwitterPassword"), this.ResponseTime);
            PollsPage polls = register.EnterRegInfo("TempTwitteruser@fake.com", "TempTwitteruser", "1/1/1980", "M", "55416");
            polls.AssertPageLoaded();
            this.ResponseTime.EndTimer("Twitter Login");
        }


        /// <summary>
        /// Login to MyVote using Twitter authentication 
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI), TestCategory(Categories.Login)]
        public void TwitterLogin()
        {
            // Open Landing Page
            LandingPage landing = new LandingPage(this.WebDriver);

            landing.OpenPage();

            //Sign in to twitter with the login credentials 
            PollsPage polls = landing.SignInExistingTwitter(Config.GetValue("TwitterUsername"), Config.GetValue("TwitterPassword"), this.ResponseTime);
            polls.AssertPageLoaded();
            this.ResponseTime.EndTimer("Twitter Login");
        }

        /// <summary>
        /// Login to MyVote using Facebook authentication
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI), TestCategory(Categories.Login)]
        public void FacebookLogin()
        {
            // Open Landing Page
            LandingPage landing = new LandingPage(this.WebDriver);
            landing.OpenPage();

            //Sign in to Facebook with the login credentials 
            PollsPage polls = landing.SignInFacebook(Config.GetValue("FacebookUsername"), Config.GetValue("FacebookPassword"), this.ResponseTime);
            polls.AssertPageLoaded();
            this.ResponseTime.EndTimer("Facebook Login");
        }

        /// <summary>
        /// Login to MyVote using Microsoft authentication
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI), TestCategory(Categories.Login)]
        public void MicrosoftLogin()
        {
            // Open Landing Page
            LandingPage landing = new LandingPage(this.WebDriver);
            landing.OpenPage();

            //Sign in to Microsoft with the login credentials
            PollsPage polls = landing.SignInMicrosoft(Config.GetValue("MicrosoftUsername"), Config.GetValue("MicrosoftPassword"), this.ResponseTime);
            polls.AssertPageLoaded();
            this.ResponseTime.EndTimer("Microsoft Login");
        }

        /// <summary>
        /// Login to MyVote using Google authentication
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI), TestCategory(Categories.Login)]
        public void GoogleLogin()
        {
            // Open Landing Page
            LandingPage landing = new LandingPage(this.WebDriver);

            landing.OpenPage();

            //Sign in to Google with the login credentials 
            PollsPage polls = landing.SignInGoogle(Config.GetValue("GoogleUsername"), Config.GetValue("GooglePassword"), this.ResponseTime);
            polls.AssertPageLoaded();

            this.ResponseTime.EndTimer("Google Login");

        }

        /// <summary>
        /// Test That access is blocked when login fails 
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI), TestCategory(Categories.Login)]
        public void BadCredentialTest()
        {
            // Open Landing Page
            LandingPage landing = new LandingPage(this.WebDriver);
            this.ResponseTime.StartTimer("Load MyVote");
            landing.OpenPage();

            //Sign in to twitter with the login credentials 
            landing.SignInBadCredentials(Config.GetValue("TwitterUsername"), Config.GetValue("TwitterPassword"));
            landing.AssertFailShows();
            this.ResponseTime.EndTimer("Load MyVote");

        }

    }
}
