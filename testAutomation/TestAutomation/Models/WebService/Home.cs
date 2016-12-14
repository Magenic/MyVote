//--------------------------------------------------
// <copyright file="Home.cs" company="Magenic">
//  Copyright 2016 Magenic, All rights Reserved
// </copyright>
// <summary>Product definition for Json</summary>
//--------------------------------------------------
using Newtonsoft.Json;

namespace Models.WebService
{
    /// <summary>
    /// Home Json
    /// </summary>
    public class Home
    {
        /// <summary>
        /// Gets or sets the home title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
