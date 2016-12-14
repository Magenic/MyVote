using System.Collections.Generic;

namespace Models.WebService
{
	public sealed class PollResponse
	{
        private int? pollID;
        private object p;
        private List<ResponseItem> responseItems;

        public PollResponse(int? pollID, int userID, string comment, List<ResponseItem> responseItems)
        {
            PollID = pollID;
            UserID = userID;
            Comment = comment;
            ResponseItems = responseItems;
        }

        public int? PollID { get; set; }
		public int UserID { get; set; }
		public string Comment { get; set; }
		public List<ResponseItem> ResponseItems { get; set; }
	}
}
