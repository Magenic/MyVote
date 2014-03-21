using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVote.AppServer.Models
{
  public class User
  {
    public string ProfileID { get; set; }
    public int? UserID { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PostalCode { get; set; }
    public string Gender { get; set; }
    public string EmailAddress { get; set; }
    public DateTime? BirthDate { get; set; }
  }
}