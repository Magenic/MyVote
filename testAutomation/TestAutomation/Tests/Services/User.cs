//--------------------------------------------------
// <copyright file="Polls.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Sample web service test class</summary>
//--------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Tests.Services
{
    /// <summary>
    /// Sample test class
    /// </summary>
    // [TestClass]
    public class User : BaseServices
    {
        /// <summary>
        /// Sample test
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetSingleUser()
        {
            User result = this.WebServiceWrapper.Get<User>("/api/user/" + TwitterUserID, "application/json", false);
        }

        /// <summary>
        /// Get info from a Poll that doesn't exist, verify 500 error message
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        [Ignore]
        public void GetSingleUserServerError()
        {
            HttpResponseMessage result = this.WebServiceWrapper.GetWithResponse("/api/user/" +  TwitterUserID, "application/json", false);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
        }

        /// <summary>
        /// Get info from a valid user, verify OK response
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void GetSingleUserResponse()
        {
            HttpResponseMessage result = this.WebServiceWrapper.GetWithResponse("/api/user/Twitter:753985978834956288", "application/json", false);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        /// <summary>
        /// Post invalid content on a single User, expecting Not Found
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void PostSingleUserBadRequest()
        {
            HttpResponseMessage result = this.WebServiceWrapper.PostWithResponse("/api/user/Google:111112107226418496236", "application/json", "{'Content': 'INVALID'}", Encoding.UTF8, "application/json", true, false);
            Assert.AreNotEqual(HttpStatusCode.OK, result.StatusCode);
        }

        /// <summary>
        /// Put invalid content on a single User, expecting Internal Server Error 500
        /// </summary>
        [TestMethod]
        [TestCategory(Categories.Service)]
        public void PutSingleUserServerError()
        {
            HttpResponseMessage result = this.WebServiceWrapper.PutWithResponse("/api/user/9999999", "application/json", "{'Content': 'INVALID'}", Encoding.UTF8, "application/json", true, false);
            Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
        }
    }
}
