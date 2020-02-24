using System;
using System.Collections.Generic;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.Domain.Entities.Extensions
{
    public static class RequestStatusNotifications
    {
        public static RequestStatusMessage BuildMessage(this TenantRequest tenantRequest)
        {
            switch (tenantRequest.RequestStatus)
            {
                case TenantRequestStatusEnum.Undefined:
                    break;
                case TenantRequestStatusEnum.RequestReceived:
                    //"Hi [TenantFullName], /r/n Your service request from [RequestDate] has been registered. Sincerely, /r/n [PropertyName] "
                    var message = new RequestStatusMessage
                    {
                        Title = "Your Request Registered",
                        Sender = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.NoReplyEmailAddress },
                        Receiver = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.ContactInfo.EmailAddress, Phone = tenantRequest.Tenant.ContactInfo.MobilePhone },
                        MessageProperties = new Dictionary<string, string>()
                        {
                            { "TenantFullName", tenantRequest.Tenant.ContactInfo.GetFullName() },
                            {  "RequestDate", tenantRequest.DateCreated.ToLongTimeString() },
                            {"PropertyName", tenantRequest.Tenant.Property.Name }
                        }
                    };
                    return message;

                case TenantRequestStatusEnum.RequestRejected:
                    //"Hi [TenantFullName], /r/n Unfortunately, we cannot start your service request from [RequestDate]. Please call us [PropertyPhone] at your convenience. /r/n  Sincerely, /r/n [PropertyName]  "
                    message = new RequestStatusMessage
                    {
                        Title = "Your Request Declined",
                        Sender = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.NoReplyEmailAddress },
                        Receiver = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.ContactInfo.EmailAddress, Phone = tenantRequest.Tenant.ContactInfo.MobilePhone },
                        MessageProperties = new Dictionary<string, string>()
                        {
                            { "TenantFullName", tenantRequest.Tenant.ContactInfo.GetFullName() },
                            {  "RequestDate", tenantRequest.DateCreated.ToLongTimeString() },
                            {"PropertyName", tenantRequest.Tenant.Property.Name }
                        }
                    };
                    return message;
                    

                case TenantRequestStatusEnum.WorkScheduled:
                    //" Hello [WorkerFullName], /r/n A new work order is scheduled for you. Please visit [WorkOrderPageUrl] for details. Sincerely, /r/n [PropertyName]"
                    message = new RequestStatusMessage
                    {
                        Title = "You have new work order",
                        Sender = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.NoReplyEmailAddress },
                        Receiver = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.ServiceWorkOrder.Person.EmailAddress , Phone = tenantRequest.ServiceWorkOrder.Person.MobilePhone  },
                        MessageProperties = new Dictionary<string, string>()
                        {
                            { "WorkerFullName", tenantRequest.ServiceWorkOrder.Person.GetFullName() },
                            {  "RequestDate", tenantRequest.DateCreated.ToLongTimeString() },
                            {  "WorkOrderPageUrl", "?"},
                            {  "PropertyName", tenantRequest.Tenant.Property.Name }
                        }
                    };
                    return message;

                case TenantRequestStatusEnum.WorkCompleted:
                    //"Hello [SuperintendentFullName], /r/n I completed work order [WorkOrderNumber]. For report, please visit [WorkReportPageUrl].  Sincerely, /r/n [WorkerFullName]"
                    message = new RequestStatusMessage
                    {
                        Title = "Work order completed",
                        Sender = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.NoReplyEmailAddress },
                        Receiver = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.Superintendent.EmailAddress, Phone = tenantRequest.Tenant.Property.Superintendent.MobilePhone  },
                        MessageProperties = new Dictionary<string, string>()
                        {
                            { "SuperintendentFullName", tenantRequest.Tenant.Property.Superintendent.GetFullName()  },
                            {  "RequestDate", tenantRequest.DateCreated.ToLongTimeString() },
                            {  "WorkOrderNumber", tenantRequest.ServiceWorkOrder.WorkOrderNo.ToString()  },
                            {  "WorkReportPageUrl", "TBD" }, 
                            { "WorkerFullName", tenantRequest.ServiceWorkOrder.Person.GetFullName() }
                        }
                    };
                    return message;

                case TenantRequestStatusEnum.WorkIncomplete:
                    //"Hello [SuperintendentFullName], /r/n Unfortunately, the work order N[WorkOrderNumber] from [WorkOrderDate] can't be completed. For the report, please visit [WorkReportPageUrl].  Sincerely, /r/n [WorkerFullName]"
                    message = new RequestStatusMessage
                    {
                        Title = "Work can't be completed",
                        Sender = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.NoReplyEmailAddress },
                        Receiver = new RequestStatusMessage.EndPointInfo() { Email = tenantRequest.Tenant.Property.Superintendent.EmailAddress, Phone = tenantRequest.Tenant.Property.Superintendent.MobilePhone },
                        MessageProperties = new Dictionary<string, string>()
                        {
                            { "SuperintendentFullName", tenantRequest.Tenant.Property.Superintendent.GetFullName()  },
                            //{  "WorkOrderDate", tenantRequest.ServiceWorkOrder.DateCreated.ToLongTimeString() },
                            {  "WorkOrderNumber", tenantRequest.ServiceWorkOrder.WorkOrderNo.ToString()  },
                            {  "WorkReportPageUrl", "TBD" },
                            { "WorkerFullName", tenantRequest.ServiceWorkOrder.Person.GetFullName() }
                        }
                    };
                    return message;
                case TenantRequestStatusEnum.Closed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new RequestStatusMessage();
        }
    }
}
