using Magenic.MaqsFramework.BaseSeleniumTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebPage;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Collections.Generic;


namespace Models
{

    public class AddPollPage : MyVoteBasePageModel
    {
        // The page URL
        private static string pageUrl = Config.GetValue("WebSiteBase") + "#/addPoll";

        #region Accessors

        // Category Dropdown field
        private static By category = By.Id("vm.newPoll.PollCategoryID");

        // Question field
        private static By question = By.Id("vm.newPoll.PollQuestion");

        // Description field
        private static By description = By.Id("vm.newPoll.PollDescription");

        // Answer fields
        private static By answer1 = By.CssSelector("div:nth-of-type(4) > input");
        private static By answer2 = By.CssSelector("div:nth-of-type(5) > input");
        private static By answer3 = By.CssSelector("div:nth-of-type(6) > input");
        private static By answer4 = By.CssSelector("div:nth-of-type(7) > input");
        private static By answer5 = By.CssSelector("div:nth-of-type(8) > input");


        // Poll image
        private static By imageUpload = By.Id("pollImage");

        // Post & View Poll
        private static By postPoll = By.CssSelector("button[class='clear-fix']");


        #endregion

        #region Methods

        public AddPollPage(IWebDriver webDriver) : base(webDriver)
        {

        }

        // Verify we are on the correct page
        public override void AssertPageLoaded()
        {
            this.webDriver.WaitForClickableElement(category);
        }
        //Selects a Category
        public void SelectCategory(string category)
        {
            var pollType = this.webDriver.WaitForClickableElement(By.Id("vm.newPoll.PollCategoryID"));
            var selectElement = new SelectElement(pollType);
            selectElement.SelectByText(category);
        }

        //Enters a question
        public void EntersQuestion(string questionStr)
        {
            this.webDriver.WaitForClickableElement(question).SendKeys(questionStr);
        }

        //Enters a question
        public void EntersDescription(string descriptionText)
        {
            this.webDriver.WaitForClickableElement(description).SendKeys(descriptionText);
        }

        //Enters in the answers
        public void EnterAnswers(string a1, string a2, string a3, string a4, string a5)
        {
            this.webDriver.WaitForClickableElement(answer1).SendKeys(a1);
            this.webDriver.WaitForClickableElement(answer2).SendKeys(a2);
            this.webDriver.WaitForClickableElement(answer3).SendKeys(a3);
            this.webDriver.WaitForClickableElement(answer4).SendKeys(a4);
            this.webDriver.WaitForClickableElement(answer5).SendKeys(a5);
        }
        /// <summary>
        /// Sends the upload path to the upload button, you nshould receive the file name and its extension and that file
        /// should be added to the solution items folder, and be located in the externals folder. 
        /// </summary>
        /// <param name="imageName"></param>
        public void UploadImage(string imageName)
        {
            string filePath = Directory.GetCurrentDirectory();
            int indexOfSteam = filePath.IndexOf("\\TestAutomation");
            filePath = filePath.Remove(indexOfSteam);
            filePath = filePath + "\\Externals\\"+ imageName;
            this.webDriver.WaitForClickableElement(imageUpload).SendKeys(filePath);
        }

        // Submit the Poll
        public ViewPollPage SubmitPoll()
        {
            this.webDriver.WaitForClickableElement(postPoll).Click();
            return new ViewPollPage(this.webDriver);
        }
        #endregion
    }
}
