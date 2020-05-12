namespace Demo.RentalRepairs.Core.Interfaces
{
    public  class EmailInfo
    {
        public string Subject { get; set;  }
        public string  RecipientEmail { get; set; }
        public string SenderEmail { get; set; }
        public string Body { get; set;  }
    }
}