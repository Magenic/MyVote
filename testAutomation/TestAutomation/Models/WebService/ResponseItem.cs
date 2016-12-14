namespace Models.WebService
{
	public sealed class ResponseItem
	{
        private int? pollOptionID;
        private bool v;

        public ResponseItem(int? pollOptionID)
        {
            this.PollOptionID = pollOptionID;
        }

        public ResponseItem(int? pollOptionID, bool isOptionSelected)
        {
            this.PollOptionID = pollOptionID;
            this.IsOptionSelected = isOptionSelected;
        }

        public int? PollOptionID { get; set; }
		public bool IsOptionSelected { get; set; }
	}
}