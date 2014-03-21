using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyVote.AppServer.Models
{
  public class PollInfo
  {
    public int? PollSubmissionID { get; set; }
    public int PollID { get; set; }
    public string PollDescription { get; set; }
    public string PollQuestion { get; set; }
    public int MaxAnswers { get; set; }
    public int MinAnswers { get; set; }
    public int UserID { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public string Comment { get; set; }
    public List<PollResponseOption> PollOptions { get; set; }
  }

  public class PollResponseOption
  {
    public int? PollResponseID { get; set; }
    public int PollOptionID { get; set; }
    public bool IsOptionSelected { get; set; }
    public short OptionPosition { get; set; }
    public string OptionText { get; set; }
  }

  public class PollResponse
  {
    public int PollID { get; set; }
    public int UserID { get; set; }
    public string Comment { get; set; }
    public List<ResponseItem> ResponseItems { get; set; }
  }

  public class ResponseItem
  {
    public int PollOptionID { get; set; }
    public bool IsOptionSelected { get; set; }
  }
}