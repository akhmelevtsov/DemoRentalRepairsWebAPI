using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Azure.Tests
{
    public class AppSettings
    {
        public string DotNetExecutablePath { get; set; }
        public string FunctionHostPath { get; set; }
        public string FunctionApplicationPath { get; set; }
        public string StorageConnectionString { get; set; }
        public string SlurpApiKey { get; set;  }
    }
}
