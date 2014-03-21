using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVote.AppServer.Models
{
  public class Poll
  {
    public int PollID { get; set; }
    public int UserID { get; set; }
    public int PollCategoryID { get; set; }
    public string PollQuestion { get; set; }
    public string PollImageLink { get; set; }
    public short PollMaxAnswers { get; set; }
    public short PollMinAnswers { get; set; }
    public DateTime? PollStartDate { get; set; }
    public DateTime? PollEndDate { get; set; }
    public bool PollAdminRemovedFlag { get; set; }
    public DateTime? PollDateRemoved { get; set; }
    public bool PollDeletedFlag { get; set; }
    public DateTime? PollDeletedDate { get; set; }
    public string PollDescription { get; set; }
    public List<PollOption> PollOptions { get; set; }
  }

  public class PollOption
  {
    public int? PollOptionID { get; set; }
    public int? PollID { get; set; }
    public short? OptionPosition { get; set; }
    public string OptionText { get; set; }
  }

  public class PollSummary
  {
    public string Category { get; set; }
    public int Id { get; set; }
    public string ImageLink { get; set; }
    public string Question { get; set; }
    public int SubmissionCount { get; set; }
  }
}