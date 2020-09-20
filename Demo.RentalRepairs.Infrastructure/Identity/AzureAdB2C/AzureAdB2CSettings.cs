using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Demo.RentalRepairs.Infrastructure.Identity.AzureAdB2C
{
    public class AzureAdB2CSettings
    {
        [JsonProperty(PropertyName = "TenantId")]
        public string TenantId { get; set; }

        [JsonProperty(PropertyName = "AppId")]
        public string AppId { get; set; }

        [JsonProperty(PropertyName = "ClientSecret")]
        public string ClientSecret { get; set; }

        [JsonProperty(PropertyName = "B2cExtensionAppClientId")]
        public string B2cExtensionAppClientId { get; set; }

        [JsonProperty(PropertyName = "UsersFileName")]
        public string UsersFileName { get; set; }

    }
}
