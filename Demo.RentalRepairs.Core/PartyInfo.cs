using System;

namespace Demo.RentalRepairs.Core
{
    public class PartyInfo
    {
        public string Id { get;   }
        public string EmailAddress { get;   }
        public string MobilePhone { get;   }

        public PartyInfo(string id, string emailAddress, string mobilePhone = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(emailAddress)) throw new ArgumentNullException(nameof(emailAddress));

            Id = id;
            EmailAddress = emailAddress;
            MobilePhone = mobilePhone;

        }
    }
}
