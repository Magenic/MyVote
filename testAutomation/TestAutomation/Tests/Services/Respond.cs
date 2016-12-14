//--------------------------------------------------
// <copyright file="Respond.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Respond web service tests</summary>
//--------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebService;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Tests.Services
{
    /// <summary>
    /// Response web service tests
    /// </summary>
    [TestClass]
    public class Respond : BaseServices
    {
        /// <summary>
        /// Sample test
        /// </summary>
       // [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetSinglePoll()
        {
            string result = this.WebServiceWrapper.Get("/api/Respond?pollID=114&userID=" + TwitterUserID, "application/xml", true);
        }

        /// <summary>
        /// Get an Invalid Single Poll
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetInvalidSinglePoll()
        {
            this.ResponseTime.StartTimer("Query Missing Response");
            HttpResponseMessage result = this.WebServiceWrapper.GetWithResponse("/api/Respond?pollID=999999", "application/json", false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
            this.ResponseTime.EndTimer("Query Missing Response");
        }

        /// <summary>
        /// Put invalid content on a single Poll, expecting Internal Server Error
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]

        public void PutSinglePollServerError()
        {
            this.ResponseTime.StartTimer("Put Invalid Response");
            HttpResponseMessage result = this.WebServiceWrapper.PutWithResponse("/api/Respond?pollID=1", "application/json", "{'Content': 'INVALID'}", Encoding.UTF8, "application/json", true, false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
            this.ResponseTime.EndTimer("Put Invalid Response");
        }
    }
}
