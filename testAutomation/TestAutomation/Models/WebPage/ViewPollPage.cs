//--------------------------------------------------
// <copyright file="ViewPollsPage.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Page object for ViewPollsPage</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;



namespace Models.WebPage
{
    /// <summary>
    /// Page object for the login page
    /// </summary>
    public class ViewPollPage : MyVoteBasePageModel
    {
        /// <summary>
        /// The page url, needs to get the actual id of the poll for this to work 
        /// </summary>
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/viewPoll/";

        /// <summary>
        /// Sample element 'By' finder
        /// </summary>
        private static By pollQuestion = By.CssSelector("h2[ng-bind='currentPoll.PollQuestion']");
        private static By pollDescription = By.CssSelector("h3[ng-bind='currentPoll.PollDescription']");

        private static By pollA1 = By.CssSelector("li:nth-child(1) > input");
        private static By pollA2 = By.CssSelector("li:nth-child(2) > input");
        private static By pollA3 = By.CssSelector("li:nth-child(3) > input");
        private static By pollA4 = By.CssSelector("li:nth-child(4) > input");
        private static By pollA5 = By.CssSelector("li:nth-child(5) > input");

        private static By submitButton = By.CssSelector("button[ng-click='submit()']");
        private static By deleteButton = By.CssSelector("button[ng-click='delete()']");

        //This selector finds the link that has an href that starts with /#/pollResult
        private static By viewResultsLink = By.CssSelector("a[href^='/#/pollResult']");
        private static By backToPollsLink = By.CssSelector("a[class='block-link'][href='/#/polls']");

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPollPage" /> class.
        /// </summary>
        /// <param name="webDriver">The selenium web driver</param>
        public ViewPollPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        /// <summary>
        /// Open the page
        /// </summary>
        public void OpenPage()
        {
            // Open the gmail login page
            this.webDriver.Navigate().GoToUrl(pageUrl);
            this.AssertPageLoaded();
        }

        /// <summary>
        /// Verify we are on the login page
        /// </summary>
        public override void AssertPageLoaded()
        {
            Assert.IsTrue(
                this.webDriver.WaitUntilVisibleElement(pollQuestion),
                "The web page '{0}' is not loaded",
                pageUrl);
        }

        /// <summary>
        /// Vote 
        /// </summary>
        public PollResultsPage VoteForAnOption(int vote)
        {
            switch (vote)
            {
                case 1:
                    this.webDriver.WaitForClickableElement(pollA1).Click();
                    break;
                case 2:
                    this.webDriver.WaitForClickableElement(pollA2).Click();
                    break;
                case 3:
                    this.webDriver.WaitForClickableElement(pollA3).Click();
                    break;
                case 4:
                    this.webDriver.WaitForClickableElement(pollA4).Click();
                    break;
                case 5:
                    this.webDriver.WaitForClickableElement(pollA5).Click();
                    break;
                default:
                    break;
            }

            this.webDriver.WaitForClickableElement(submitButton).Click();
            return new PollResultsPage(this.webDriver);
        }

        /// <summary>
        /// Return the URL 
        /// </summary>
        public string GetURL()
        {
            // Return the URL
            return this.webDriver.Url.ToString();
        }

        /// <summary>
        /// Click the view results link
        /// </summary>
        public PollsPage ClickBackToPollsLink()
        {
            // Address race condition
            this.webDriver.WaitForPageLoad();
            this.webDriver.WaitForClickableElement(backToPollsLink).Click();
 
            return new PollsPage(this.webDriver);
        }

        /// <summary>
        /// Click the view results link
        /// </summary>
        public PollResultsPage ClickViewResults()
        {
            this.webDriver.WaitForClickableElement(viewResultsLink).Click();
            return new PollResultsPage(this.webDriver);
        }
    }
}
