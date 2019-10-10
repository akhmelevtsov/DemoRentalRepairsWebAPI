using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

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

        public static TenantRequestDocModel BuildModel(this TenantRequestDoc tenantRequest)
        {
            return new TenantRequestDocModel()
            { 
                RequestItems = tenantRequest.RequestItems 
            };
        }

        public static TenantRequestModel BuildModel(this TenantRequest tenantRequest)
        {
            return new TenantRequestModel()
            {
                RequestStatus = tenantRequest.RequestStatus,
                RejectNotes = tenantRequest.RejectNotes,
                RequestDoc = tenantRequest.RequestDoc,
                ServiceWorkOrder = tenantRequest.ServiceWorkOrder,
                WorkReport = tenantRequest.WorkReport

            };
        }
    }
}
