using Demo.RentalRepairs.Core.Enums;

namespace Demo.RentalRepairs.Core
{
    public class PropertyMessage 
    {
        public PropertyMessageTypeEnum MessageType { get; }
        public PartyInfo Sender { get; }
        public PartyInfo Recipient { get; }
     

        public PropertyMessage(PropertyMessageTypeEnum messageType, PartyInfo sender, PartyInfo recipient)
        {
            MessageType = messageType;
            Sender = sender;
            Recipient = recipient;
           
        }

    }
}
