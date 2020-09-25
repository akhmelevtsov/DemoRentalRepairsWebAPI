using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Framework;
using Demo.RentalRepairs.Domain.ValueObjects;

namespace Demo.RentalRepairs.Infrastructure.Repositories.AzureTableApi.TableEntity
{
    public class PropertyTbl : Microsoft.Azure.Cosmos.Table.TableEntity
    {
        public const string PartitionKeyConst = "Property";
        public PropertyTbl(string propertyCode)
        {
            PartitionKey = PartitionKeyConst;
            RowKey = propertyCode;
        }

        public PropertyTbl()
        {
            
        }
        public string ID => RowKey; // property code

        public string Name { get;  set; }
       
       
        public string Address { get;  set; }
        public string PhoneNumber { get;  set; }
       
        public string Superintendent { get;  set; }
        public string NoReplyEmailAddress { get; set; }

        public string Units { get;  set; } 

        public DateTime DateCreated { get; set;  }
        public Guid IdCreated { get; set;  }

    }
}
