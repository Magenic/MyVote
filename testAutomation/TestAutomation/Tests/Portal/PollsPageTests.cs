//--------------------------------------------------
// <copyright file="PollsPageTests.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Sample Selenium test class</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using System;
using Models;
using Models.WebPage;
using OpenQA.Selenium;

// TODO: Add reference to object model
// using PageModel;

namespace Tests.Portal
{
    /// <summary>
    /// Tests that cover the functionality of the Polls page 
    /// </summary>
    [TestClass]
    public class PollsPageTests : BaseSeleniumTest
    {
        /// <summary>
        /// TODO - Make this more generic
        /// </summary>
        /// <returns></returns>
        private PollsPage LoginAndNavigateToPolls()
        {
            LandingPage landing = new LandingPage(this.WebDriver);
            landing.OpenPage();

            //Sign in to twitter with the login credentials 
            PollsPage polls = landing.SignInExistingTwitter(Config.GetValue("TwitterUsername"), Config.GetValue("TwitterPassword"));
            polls.AssertPageLoaded();

            return polls;
        }

        /// <summary>
        /// Sort poll categories by Most Popular
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void SortByMostPopular()
        {
            
            PollsPage polls = LoginAndNavigateToPolls();
            polls.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Page Timer");
            polls.SortPollsBy("Most Popular");
            this.ResponseTime.EndTimer("Polls Page Timer");
        }

        /// <summary>
        /// Sort poll categories by Newest
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void SortByNewest()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            polls.AssertPageLoaded();
            
            polls.SortPollsBy("Most Popular");
            this.ResponseTime.StartTimer("Polls Page Timer");
            polls.SortPollsBy("Newest");
            this.ResponseTime.EndTimer("Polls Page Timer");
            
        }

        /// <summary>
        /// Logout From Polls Page
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void LogoutFromPolls()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            polls.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Page Timer");
            LandingPage landing = polls.Logout();
            landing.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Page Timer");
        }

        /// <summary>
        /// Logout From Polls Page
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void ConfirmHomepageIconReturns()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            polls.AssertPageLoaded();
  
            AddPollPage newPole = polls.StartNewPoll();
            this.ResponseTime.StartTimer("Polls Page Timer");
            PollsPage pollsAgain = newPole.HomepageIconClick();
            pollsAgain.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Page Timer");
        }

        /// <summary>
        /// Confirm Add Poll Button
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void VerifyAddPollButton()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            polls.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Page Timer");
            AddPollPage newPole = polls.StartNewPoll();
            newPole.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Page Timer");
        }

        /// <summary>
        /// Open existing First pole
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void OpenAnExistingPoll()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            polls.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Page Timer");
            ViewPollPage firstPole = polls.OpenFirstPoll();
            this.ResponseTime.EndTimer("Polls Page Timer");
        }
    }
}
