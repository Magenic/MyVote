//--------------------------------------------------
// <copyright file="PollTests.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Sample Selenium test class</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseSeleniumTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Magenic.MaqsFramework.Utilities.Helper;
using Models;
using Models.WebPage;


using Models.WebService;
using OpenQA.Selenium;
using Tests.Portal;
using System;

namespace Tests
{
    /// <summary>
    /// Test class for tests about polls
    /// </summary>
    [TestClass]
    public class PollTests : BaseSeleniumTest 
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
        /// Open existing First pole
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void VerifyViewResultsLink()
        { 
            PollsPage PP = LoginAndNavigateToPolls();
            AddPollPage addPage = PP.StartNewPoll();
            addPage.SelectCategory("Technology");
            addPage.EntersQuestion("What is an automated test?");
            addPage.EntersDescription("This question is about test automation and trying to gage your thought process");
            addPage.EnterAnswers("Automated tests are pointless", "Automated tests are time savers", "Automated tests are invaluable", "Ehhhhh they are okay", "I don't know how to automate, so they are magic");
            //addPage.UploadImage("test.jpg");
            ViewPollPage views = addPage.SubmitPoll();
            views.AssertPageLoaded();

            this.ResponseTime.StartTimer("Polls Timer");
            PollResultsPage results = views.ClickViewResults();
            results.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// Verify back to Polls link works 
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void VerifyViewBackToPollsLink()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            ViewPollPage firstPole = polls.OpenFirstPoll();
            firstPole.AssertPageLoaded();

            this.ResponseTime.StartTimer("Polls Timer");
            polls = firstPole.ClickBackToPollsLink();
            polls.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void VerifyHomepageIconFromPoll()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            ViewPollPage firstPole = polls.OpenFirstPoll();
            firstPole.AssertPageLoaded();

            this.ResponseTime.StartTimer("Polls Timer");
            polls = firstPole.HomepageIconClick();
            polls.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void VerifyLogoutFromPoll()
        {
            PollsPage polls = LoginAndNavigateToPolls();
            ViewPollPage firstPole = polls.OpenFirstPoll();
            firstPole.AssertPageLoaded();

            this.ResponseTime.StartTimer("Polls Timer");
            LandingPage landing = firstPole.Logout();
            landing.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// Add Poll
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void AddPoll()
        {
            PollsPage page = LoginAndNavigateToPolls();
            page.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Timer");

            //Start a new poll
            AddPollPage addPage =  page.StartNewPoll();

            addPage.SelectCategory("Technology");
            addPage.EntersQuestion("What is an automated test?");
            addPage.EntersDescription("This question is about test automation and trying to gage your thought process");
            addPage.EnterAnswers("Automated tests are pointless", "Automated tests are time savers","Automated tests are invaluable","Ehhhhh they are okay","I don't know how to automate, so they are magic");

            //Upload an image, the variable that gets passed in is the file name with the file extension ex. test.jpg
            //Make sure that the image is added to the Solution items which is in the Externals folder
            //addPage.UploadImage("test.jpg");

            ViewPollPage views = addPage.SubmitPoll();
            views.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// Vote on a poll
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void VoteForPoll()
        {
            PollsPage PP = LoginAndNavigateToPolls();
            AddPollPage addPage = PP.StartNewPoll();
            addPage.SelectCategory("Technology");
            addPage.EntersQuestion("What is an automated test? " +  Guid.NewGuid());
            addPage.EntersDescription("This question is about test automation and trying to gage your thought process");
            addPage.EnterAnswers("Automated tests are pointless", "Automated tests are time savers", "Automated tests are invaluable", "Ehhhhh they are okay", "I don't know how to automate, so they are magic");
            //addPage.UploadImage("test.jpg");
            ViewPollPage views = addPage.SubmitPoll();
            views.AssertPageLoaded();

            this.ResponseTime.StartTimer("Polls Timer");
            PollResultsPage results = views.VoteForAnOption(1);
            results.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// Vote on a poll
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void TryToSubmitVoteWithoutVoting()
        {
            PollsPage PP = LoginAndNavigateToPolls();
            AddPollPage addPage = PP.StartNewPoll();
            addPage.SelectCategory("Technology");
            addPage.EntersQuestion("What is an automated test?");
            addPage.EntersDescription("This question is about test automation and trying to gage your thought process");
            addPage.EnterAnswers("Automated tests are pointless", "Automated tests are time savers", "Automated tests are invaluable", "Ehhhhh they are okay", "I don't know how to automate, so they are magic");
            //addPage.UploadImage("test.jpg");
            ViewPollPage views = addPage.SubmitPoll();
            views.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Timer");

            PollResultsPage results = views.VoteForAnOption(1);
            views.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }

        /// <summary>
        /// Vote on a poll
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.UI)]
        public void SubmitMultipleVoteOptions()
        {
            
            PollsPage PP = LoginAndNavigateToPolls();
            AddPollPage addPage = PP.StartNewPoll();
            addPage.SelectCategory("Technology");
            addPage.EntersQuestion("What is an automated test?");
            addPage.EntersDescription("This question is about test automation and trying to gage your thought process");
            addPage.EnterAnswers("Automated tests are pointless", "Automated tests are time savers", "Automated tests are invaluable", "Ehhhhh they are okay", "I don't know how to automate, so they are magic");
            //addPage.UploadImage("test.jpg");
            ViewPollPage views = addPage.SubmitPoll();
            views.AssertPageLoaded();
            this.ResponseTime.StartTimer("Polls Timer");

            PollResultsPage results = views.VoteForAnOption(1);
            views.AssertPageLoaded();
            this.ResponseTime.EndTimer("Polls Timer");
        }
    }
}
