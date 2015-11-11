using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Mvc5AppServer.Controllers
{
  public class DataPortalController : Csla.Server.Hosts.HttpPortalController
  {
    public async override Task<HttpResponseMessage> PostAsync(string operation)
    {
      var result = await base.PostAsync(operation).ConfigureAwait(false);
      return result;
    }
  }
}