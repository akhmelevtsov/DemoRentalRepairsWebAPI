using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.RentalRepairs.Infrastructure.Identity.AzureAdB2C
{
    internal class B2CCustomAttributeHelper
    {
        internal readonly string B2CExtensionAppClientId;

        internal B2CCustomAttributeHelper(string b2cExtensionAppClientId)
        {
            B2CExtensionAppClientId = b2cExtensionAppClientId.Replace("-", "");
        }

        internal string GetCompleteAttributeName(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                throw new System.ArgumentException("Parameter cannot be null", nameof(attributeName));
            }

            return $"extension_{B2CExtensionAppClientId}_{attributeName}";
        }
    }
}
