//--------------------------------------------------
// <copyright file="PollResultsPage.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Page object for PollResultsPage</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseSeleniumTest;
using OpenQA.Selenium;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Models.WebPage
{
    /// <summary>
    /// Page object for the login page
    /// </summary>
    public class PollResultsPage : MyVoteBasePageModel
    {
        /// <summary>
        /// The page url
        /// </summary>
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/pollResult/";

        /// <summary>
        /// Sample element 'By' finder
        /// </summary>
        private static By pollImage = By.CssSelector(".poll-result-img");
        private static By votesLabel = By.CssSelector(".poll-result-details>h3");
        private static By pollCommentTextArea = By.CssSelector(".poll-new-comment");
        private static By pollCommentButtom = By.CssSelector(".poll-new-comment-button");
        private static By deletePollButton = By.CssSelector(".left-view.ng-scope>button");

        //These selectors will grab multiple elements if there are multiple comments and replies
        private static By pollCommentsBox = By.CssSelector(".poll-result-top-comment.ng-scope");
        private static By pollCommentsText= By.CssSelector(".comment-text.ng-binding");
        private static By pollCommentsReplyTextarea = By.CssSelector(".poll-new-comment-input.ng-pristine.ng-untouched.ng-valid.ng-empty");

        /// <summary>
        /// Initializes a new instance of the <see cref="PollResultsPage" /> class.
        /// </summary>
        /// <param name="webDriver">The selenium web driver</param>
        public PollResultsPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        /// <summary>
        /// Open the page
        /// </summary>
        public  void OpenPage()
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
                this.webDriver.WaitUntilVisibleElement(votesLabel),
                "The web page '{0}' is not loaded",
                pageUrl);
        }
    }
}
