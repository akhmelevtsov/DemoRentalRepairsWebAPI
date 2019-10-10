﻿using System.Collections.Generic;

namespace Demo.RentalRepairs.Domain.ValueObjects.Request
{
    public class RequestStatusMessage
    {
        public RequestStatusMessage()
        {
            MessageProperties = new Dictionary<string, string>();
        }
        public EndPointInfo  Sender { get; set; }
        public EndPointInfo  Receiver { get; set;  }

        public string  Message { get; set; }
        public Dictionary<string,string> MessageProperties { get; set;  }
        public string Title { get; set; }

        public class EndPointInfo
        {
            public string  Email { get; set; }
            public string Phone { get; set; }
        }


    }
}
