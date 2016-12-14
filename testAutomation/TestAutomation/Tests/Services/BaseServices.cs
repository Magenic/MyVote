//--------------------------------------------------
// <copyright file="BaseServices.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Base setup class for web service tests</summary>
//--------------------------------------------------
using Magenic.MaqsFramework.BaseDatabaseTest;
using Magenic.MaqsFramework.BaseWebServiceTest;
using Magenic.MaqsFramework.Utilities.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models.WebService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tests.Services
{
    /// <summary>
    /// Base for web service tests
    /// </summary>
    [TestClass]
    public class BaseServices : BaseWebServiceTest
    {
        private static string CachedToken;
        private static DateTime TokenExpires = DateTime.MinValue;
        protected static int TwitterUserID;


        /// <summary>
        /// Override web service test setup
        /// </summary>
        /// <param name="baseAddress">The base service address</param>
        /// <param name="mediaType">The service media type</param>
        /// <returns></returns>
        protected override HttpClient GetHttpClient(Uri baseAddress, string mediaType)
        {
            return GetClient(baseAddress, mediaType);
        }

        /// <summary>
        /// Sample test
        /// </summary>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // Do one time setup via database
            using (DatabaseConnectionWrapper wrapper = new DatabaseConnectionWrapper(DatabaseConfig.GetConnectionString()))
            {

               DataTable table =  wrapper.QueryAndGetDataTable("SELECT [UserID] FROM [MVUser] WHERE [ProfileID]='Twitter:753985978834956288'");
                TwitterUserID = (int)table.Rows[0][0];

                // Cleanup old polls
                HttpClientWrapper client = new HttpClientWrapper(new Uri(WebServiceConfig.GetWebServiceUri()));
                client.BaseHttpClient = GetClient(new Uri(WebServiceConfig.GetWebServiceUri()), "application/json");

                foreach (PollSummary poll in client.Get<List<PollSummary>>("/api/poll", "application/json"))
                {
                    if (poll.Question.StartsWith("What is an automated test") || poll.Question.StartsWith("What is Test Automation?") || poll.Question.StartsWith("ponies?") || poll.Question.StartsWith("Ringooo"))
                    {
                        SqlParameter pollId = new SqlParameter("PollID", poll.Id);
                        int ned = wrapper.RunActionProcedure("[dbo].[deletePoll]", pollId);
                    }
                }
            }
        }

        private static HttpClient GetClient(Uri baseAddress, string mediaType)
        {
            HttpClient customClient = new HttpClient();

            // Override default and use temp location for now
            customClient.BaseAddress = new Uri(WebServiceConfig.GetWebServiceUri());
            customClient.DefaultRequestHeaders.Accept.Clear();
            customClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            HttpClientWrapper client = new HttpClientWrapper(new Uri(WebServiceConfig.GetWebServiceUri()));
            client.BaseHttpClient = customClient;

            customClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken(baseAddress));

            return customClient;
        }


        private static string GetToken(Uri baseAddress)
        {
            if (string.IsNullOrEmpty(CachedToken) || TokenExpires.Ticks < DateTime.Now.Ticks)
            {
                string configKey = Config.GetValue("TempWSKey", "");

                if(string.IsNullOrEmpty(configKey))
                {
                    return PullFromService(baseAddress);
                }
                else
                {
                    CachedToken = Encoding.UTF8.GetString(Convert.FromBase64String(configKey));
                    TokenExpires = DateTime.MaxValue;
                }
            }

            return CachedToken;
        }

        private static string PullFromService(Uri baseAddress)
        {

            HttpClient customClient = new HttpClient();

            // Override default and use temp location for now
            customClient.BaseAddress = new Uri(WebServiceConfig.GetWebServiceUri());
            customClient.DefaultRequestHeaders.Accept.Clear();
            customClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var formContent = new FormUrlEncodedContent(new[]
            {

                new KeyValuePair<string, string>(Config.GetValue("APILogin"), Config.GetValue("APILoginName")),
                new KeyValuePair<string, string>(Config.GetValue("APIPassword"), Config.GetValue("APIPasswordValue"))
            });

            HttpClientWrapper client = new HttpClientWrapper(new Uri(WebServiceConfig.GetWebServiceUri()));
            client.BaseHttpClient = customClient;


            Token tokenResponse = JsonConvert.DeserializeObject<Token>(client.Post("api/token", "application/json", formContent));


            CachedToken = tokenResponse.access_token;
            TokenExpires = DateTime.Now.AddSeconds(tokenResponse.expires_in - 1);

            return CachedToken;
        }

    }
}
