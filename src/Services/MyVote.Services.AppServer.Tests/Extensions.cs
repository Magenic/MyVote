using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MyVote.Services.AppServer.Tests
{
  public static class Extensions
  {
    public static void DumpException(this HttpResponseException ex)
    {
      Console.WriteLine(ex.ToString());
      Console.WriteLine();
      Console.WriteLine("Web response:");
      Console.WriteLine(ex.Response);

      Console.WriteLine();
      Console.WriteLine("Web response content:");
      var content = ex.Response.Content;
      Console.WriteLine(content.ReadAsStringAsync().Result);
    }
  }
}
