namespace Models.WebService
{
	public sealed class PollOption
	{

        public PollOption(int? v1, int? v2, short? v3, string v4, bool v5)
        {
            this.PollOptionID = v1;
            this.PollID = v2;
            this.OptionPosition = v3;
            this.OptionText = v4;
            this.Selected = v5;
        }

        public int? PollOptionID { get; set; }
		public int? PollID { get; set; }
		public short? OptionPosition { get; set; }
		public string OptionText { get; set; }
        public bool Selected { get; set; }
    }
}
