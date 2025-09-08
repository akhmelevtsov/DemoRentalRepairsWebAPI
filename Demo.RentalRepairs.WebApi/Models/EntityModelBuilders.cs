using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using System.Text.RegularExpressions;

namespace Demo.RentalRepairs.WebApi.Models
{
    public static class EntityModelBuilders
    {
        public static PropertyModel BuildModel(this Property property)
        {
            return new PropertyModel()
            {
                Code = property.Code, Name = property.Name,
                Address = property.Address,
                Superintendent = property.Superintendent,
                PhoneNumber = property.PhoneNumber,
                Units = property.Units 
            };
        }
        public static TenantModel  BuildModel(this Tenant tenant)
        {
            return new TenantModel()
            {
                ContactInfo = tenant.ContactInfo , UnitNumber = tenant.UnitNumber 
                 
            };
        }

        public static TenantRequestDocModel BuildModel(this RegisterTenantRequestCommand tenantRequest)
        {
            return new TenantRequestDocModel()
            { 
                Title = tenantRequest.Title 
            };
        }

        public static TenantRequestModel BuildModel(this TenantRequest tenantRequest)
        {
            return new TenantRequestModel()
            {
                RequestCode = tenantRequest.Code,
                RequestStatus = SplitPascalCase(tenantRequest.RequestStatus.ToString()),
                DateCreated = tenantRequest.DateCreated ,
                RequestTitle = tenantRequest.RequestTitle ,
            };
        }

        private static string SplitPascalCase(string input)
        {
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
    }
}
