using System;
using System.Collections.Generic;
using System.Text;
using Demo.RentalRepairs.Core.Interfaces;
using Microsoft.Azure.Cosmos.Table;

namespace Demo.RentalRepairs.Infrastructure.Notifications.Models
{
    public class EmailInfoModel : EmailInfo
    {
 
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }
    }
}
