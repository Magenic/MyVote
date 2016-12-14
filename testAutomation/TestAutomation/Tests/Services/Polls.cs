//--------------------------------------------------
// <copyright file="Polls.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Sample web service test class</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseWebServiceTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Tests.Services
{
    /// <summary>
    /// Sample test class
    /// </summary>
    [TestClass]
    public class Polls : BaseServices
    {
        /// <summary>
        /// Sample test
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetListOfPolls()
        {
            this.ResponseTime.StartTimer("List of Polls");
            List<PollSummary> result = this.WebServiceWrapper.Get<List<PollSummary>>("/api/poll", "application/json");
            this.ResponseTime.EndTimer("List of Polls");
        }

        /// <summary>
        /// Sample test
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetSinglePoll()
        {
            this.ResponseTime.StartTimer("Single Poll");
            Poll result = this.WebServiceWrapper.Get<Poll>("/api/poll/1", "application/json");
            this.ResponseTime.EndTimer("Single Poll");
        }


        /// <summary>
        /// Grab all none-deleted polls in order of most popular by category
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetPollsByMostPopular()
        {
            this.ResponseTime.StartTimer("Most Popular");
            List<PollSummary> result = this.WebServiceWrapper.Get<List<PollSummary>>("/api/Poll?filterBy=MostPopular", "application/json");
            Assert.IsTrue(result[0].SubmissionCount >= result[1].SubmissionCount);
            this.ResponseTime.EndTimer("Most Popular");
        }

        /// <summary>
        /// Grab all none-deleted polls in order of newest by category
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetPollsByNewest()
        {
            this.ResponseTime.StartTimer("Newest Polls");
            List<PollSummary> result = this.WebServiceWrapper.Get<List<PollSummary>>("/api/Poll?filterBy=Newest", "application/json");
            this.ResponseTime.EndTimer("Newest Polls");
        }

        /// <summary>
        /// Create a poll
        /// </summary>
        //[TestMethod]
        [TestCategory(Categories.Service)]
        public void CreatePoll()
        {
            List<PollOption> pollOptions = new List<PollOption>();
            pollOptions.Add(new PollOption(null, null, 0, "test", false));
            pollOptions.Add(new PollOption(null, null, 1, "test", false));

            ///the old azure instance sends in null for the PollID, but the new google instance needs to have a non null int. Comments from Josh:
            ///"it needs to be 0, that was just a recent find. basically the .net core version seems to be more strict on what it accepts, because 
            ///its suppost to be an int for that field"
            ///

            int? test = 0;
#if Azure
            test = null;
#endif



            Poll poll1 = new Poll(test, TwitterUserID, 2, "Ringoooooo", null, 1, 1, Convert.ToDateTime("1753-01-01T06:00:00.000Z"), Convert.ToDateTime("9999-12-31T17:59:59.999Z"), false, null, false, null, "test", pollOptions);
            var content = WebServiceUtils.MakeStringContent<Poll>(poll1, Encoding.UTF8, "application/json");

            this.ResponseTime.StartTimer("Create Poll");
            var createdPoll = this.WebServiceWrapper.Put("/api/Poll", "application/json", content, true);
            this.ResponseTime.EndTimer("Create Poll");

        }

      //  [TestMethod]
        [TestCategory(Categories.Service)]
        public void DeletePoll()
        {
            //Create a poll to test delete
            Poll poll = CreateAndReturnPoll();

            this.WebServiceWrapper.Delete<Poll>("/api/Poll/" + poll.PollID, "application/json", true);
        }

        /// <summary>
        /// Vote for a poll
        /// </summary>
        //[TestMethod]
        [TestCategory(Categories.Service)]
        public void VoteForAPoll()
        {

           Poll poll = CreateAndReturnPoll();
            List <PollOption> options = poll.PollOptions;
            List<ResponseItem> responseItems = new List<ResponseItem>();
            int choice = 1; 
            for (int x = 0; x < options.Count; x++)
            {
                if (x == choice)
                {
                    responseItems.Add(new ResponseItem(options[x].PollOptionID,true));
                }
                else
                {
                    responseItems.Add(new ResponseItem(options[x].PollOptionID));
                }
            }
            
            PollResponse response = new PollResponse(poll.PollID, poll.UserID, null, responseItems);
            var content = WebServiceUtils.MakeStringContent<PollResponse>(response, Encoding.UTF8, "application/json");

            this.ResponseTime.StartTimer("Vote for a Poll");
            this.WebServiceWrapper.Put<PollResponse>("/api/Respond", "application/json", content, true);
            this.ResponseTime.EndTimer("Vote for a Poll");
        }


        /// <summary>
        /// Get info from a Poll that doesn't exist, verify 500 error message
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]

        public void GetSinglePollServerError()
        {
            this.ResponseTime.StartTimer("Query Missing Poll");
            HttpResponseMessage result = this.WebServiceWrapper.GetWithResponse("/api/poll/999999", "application/json", false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
            this.ResponseTime.EndTimer("Query Missing Poll");
        }

        /// <summary>
        /// Get info from a URL that doesn't exist, verify 404 error message
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetSinglePollNotFound()
        {
            this.ResponseTime.StartTimer("Invalid Poll Query");
            HttpResponseMessage result = this.WebServiceWrapper.GetWithResponse("/api/polls/0", "application/json", false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
            this.ResponseTime.EndTimer("Invalid Poll Query");
        }

        /// <summary>
        /// Put invalid content on a single Poll, expecting bad request
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void PutSinglePollBadRequest()
        {
            this.ResponseTime.StartTimer("Invalid Poll Put");
            HttpResponseMessage result = this.WebServiceWrapper.PutWithResponse("/api/poll", "application/json", "{'Content': 'INVALID'}", Encoding.UTF8, "application/json", true, false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
            this.ResponseTime.EndTimer("Invalid Poll Put");
        }

        /// <summary>
        /// Put invalid content on a single Poll, expecting bad request
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void DeleteSinglePollServerError()
        {
            this.ResponseTime.StartTimer("Invalid Poll Delete");
            HttpResponseMessage result = this.WebServiceWrapper.DeleteWithResponse("/api/poll/999999", "application/json", false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
            this.ResponseTime.EndTimer("Invalid Poll Delete");
        }

        /// <summary>
        /// Delete a poll as cleanup
        /// </summary>
        /// <param name="pollId"></param>
        public void DeleteCreatedPoll(string pollId)
        {
            //Try catch cause google instance currently fails to delete
            try
            {
                this.WebServiceWrapper.Delete<Poll>(String.Format("/api/Poll?id={0}", pollId), "application/json", true);
            }
            catch { }
        }

        /// <summary>
        /// Create a Poll to be used for testing
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public Poll CreateAndReturnPoll()
        {
            string name = CreatePollSetup();

            foreach (PollSummary summary in  this.WebServiceWrapper.Get<List<PollSummary>>("/api/Poll?filterBy=Newest", "application/json"))
            {
                if(name.Equals(summary.Question))
                {
                    return this.WebServiceWrapper.Get<Poll>("/api/poll/" + summary.Id, "application/json");
                }
            }

            return null;
        }


        public string CreatePollSetup()
        {
            string testName = "What is Test Automation? " + Guid.NewGuid();

            List<PollOption> pollOptions = new List<PollOption>();
            pollOptions.Add(new PollOption(null, null, 0, "Magic", false));
            pollOptions.Add(new PollOption(null, null, 1, "Technology", false));
            pollOptions.Add(new PollOption(null, null, 1, "Time Saver ", false));
            pollOptions.Add(new PollOption(null, null, 1, "Power", false));
            pollOptions.Add(new PollOption(null, null, 1, "Terrible", false));

            int? test = 0;
#if Azure
            test = null;
#endif


            Poll poll1 = new Poll(test, TwitterUserID, 1, testName, null, 1, 1, DateTime.Now, DateTime.Now.AddDays(100), false, null, false, null, "test", pollOptions);
            var content = WebServiceUtils.MakeStringContent<Poll>(poll1, Encoding.UTF8, "application/json");
            Poll createdPoll = this.WebServiceWrapper.Put<Poll>("/api/Poll", "application/json", content, true);

            return testName;
        }

    }
}
