//--------------------------------------------------
// <copyright file="MyVoteBasePageModel.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Page object for MyVoteBasePageModel</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseSeleniumTest;
using OpenQA.Selenium;

namespace Models.WebPage
{
    /// <summary>
    /// Page object for the login page
    /// </summary>
    public abstract class MyVoteBasePageModel
    {

        /// <summary>
        /// Elements that appear on ever page of the myVote site
        /// </summary>
        private static By myVoteHeader = By.CssSelector("h1 > a[href='/#/polls']");
        private static By signInAuth = By.CssSelector("div > span[class='ng - binding']");
        private static By logoutBtn = By.CssSelector(".logout-link");

        /// <summary>
        /// Selenium Web Driver
        /// </summary>
        protected IWebDriver webDriver;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyVoteBasePageModel" /> class.
        /// </summary>
        /// <param name="webDriver">The selenium web driver</param>
        protected MyVoteBasePageModel(IWebDriver webDriver)
        {
            this.webDriver = webDriver;
        }

        /// <summary>
        /// Verify we are on the login page
        /// </summary>
        public abstract void AssertPageLoaded();

        /// <summary>
        /// Click the logout button
        /// </summary>
        /// <returns></returns>
        public LandingPage Logout()
        {
            this.webDriver.WaitForClickableElement(logoutBtn).Click();
            return new LandingPage(this.webDriver);
        }
        /// <summary>
        /// Click the hompepage Icon
        /// </summary>
        /// <returns></returns>
        public PollsPage HomepageIconClick()
        {
            this.webDriver.WaitForClickableElement(myVoteHeader).Click();
            return new PollsPage(this.webDriver);
        }


    }
}
