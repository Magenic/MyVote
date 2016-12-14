using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyVote.Services.AppServer.Controllers
{
    [Route("api/[controller]")]
    public class DataPortalController : Csla.Server.Hosts.HttpPortalController
    {
        [HttpPost]
        public async override Task PostAsync(string operation)
        {
            await base.PostAsync(operation).ConfigureAwait(false);
        }
    }
}