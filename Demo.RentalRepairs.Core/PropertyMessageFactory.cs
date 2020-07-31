using System;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using Demo.RentalRepairs.Core.Enums;
using Demo.RentalRepairs.Core.Interfaces;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.Exceptions;

namespace Demo.RentalRepairs.Core
{
    public  class PropertyMessageFactory
    {
        private readonly ITemplateDataService _templateDataService;
        private readonly TenantRequest _tenantRequest;
        private readonly IPropertyRepository _propertyRepository;
        private Worker _worker;

        public PropertyMessageFactory(ITemplateDataService templateDataService, TenantRequest tenantRequest, IPropertyRepository propertyRepository)
        {
            _templateDataService = templateDataService;
            _tenantRequest = tenantRequest;
            _propertyRepository = propertyRepository;
        }

        public EmailInfo CreateTenantRequestEmail()
        {
            if (_tenantRequest.RequestStatus == TenantRequestStatusEnum.Scheduled
                || _tenantRequest.RequestStatus == TenantRequestStatusEnum.Done
                || _tenantRequest.RequestStatus == TenantRequestStatusEnum.Failed)
            {
  
                    _worker = _propertyRepository.GetWorkerByEmail(_tenantRequest.WorkerEmail);
                    if (_worker == null)
                        throw new DomainEntityNotFoundException("worker_not_found", "Worker not found");

            }
            var message = CreateMessageOnStatusChange();
            return message == null ? null : CreateEmail(message);
        }

        public  EmailInfo CreateEmail( PropertyMessage m)
        {
            
            var e = new EmailInfo()
            {
                Subject = CreateSubject(m.MessageType),
                RecipientEmail = m.Recipient.EmailAddress, 
                SenderEmail = m.Sender.EmailAddress,
                Body = CreateBody(m.MessageType)
            };
            return e;
        }
        private  string CreateSubject(PropertyMessageTypeEnum messageType)
        { 
            var template = _templateDataService.GetString( GetSubjectTemplateName(messageType));
            if (template == null)
                throw new NullReferenceException(nameof(template));
            return template;
        }

        private  string CreateBody(PropertyMessageTypeEnum messageType)
        {

            string bodyTemplate = _templateDataService.GetString(GetBodyTemplateName(messageType));
            
            if (bodyTemplate == null)
                throw new NullReferenceException(nameof(bodyTemplate));

            foreach (Match m in  Regex.Matches(bodyTemplate, @"\[.*?\]"))
            {
                var placeholder = m.Value;
                var parameterStr= placeholder.Trim('[', ']');
                if (!Enum.TryParse(parameterStr, true, out PropertyMessageParamsEnum parameter))
                        throw new InvalidEnumArgumentException(nameof(parameterStr));
                string value = GetParameterValue(parameter);
                if (string.IsNullOrEmpty( value))
                    throw new NoNullAllowedException($"Empty value for placeholder {placeholder}");

                bodyTemplate = bodyTemplate.Replace(placeholder, value);
            }

            return bodyTemplate;

        }

        private string GetParameterValue(PropertyMessageParamsEnum parameter)
        {
            switch (parameter )
            {
                case PropertyMessageParamsEnum.TenantFullName:
                    return _tenantRequest.TenantFullName; 
                case PropertyMessageParamsEnum.RequestDate:
                    return _tenantRequest.RequestDate;
                case PropertyMessageParamsEnum.PropertyName:
                    return _tenantRequest.PropertyName;
                case PropertyMessageParamsEnum.WorkerFullName:
                    return _worker.PersonContactInfo.GetFullName() ;
                case PropertyMessageParamsEnum.SuperintendentFullName:
                    return _tenantRequest.SuperintendentFullName;
                case PropertyMessageParamsEnum.WorkOrderNumber:
                    return _tenantRequest.WorkOrderNumber; 
                case PropertyMessageParamsEnum.PropertyPhone:
                    return _tenantRequest.PropertyPhone;
                case PropertyMessageParamsEnum.WorkOrderDate:
                    return _tenantRequest.WorkOrderDate;
                case PropertyMessageParamsEnum.WorkOrderPageUrl:
                    return "--WorkOrderPageUrl--";
                case PropertyMessageParamsEnum.WorkReportPageUrl:
                    return "--WorkReportPageUrl--";
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter), parameter, "");
            }
        }

        private string GetSubjectTemplateName(PropertyMessageTypeEnum messageType)
        {
            return messageType.ToString() + "SubjectTemplate";
        }
        private string GetBodyTemplateName(PropertyMessageTypeEnum messageType)
        {
            return messageType + "Template";
        }
        private  PropertyMessage CreateMessageOnStatusChange()
        {
            
            switch (_tenantRequest.RequestStatus)
            {
                case TenantRequestStatusEnum.Undefined:
                    break;
                case TenantRequestStatusEnum.Submitted:
                    //"Hi [TenantFullName], /r/n Your service request from [RequestDate] has been registered. Sincerely, /r/n [PropertyName] "
                    //Title = "Your Request Registered",
                    return new PropertyMessage
                    (
                        PropertyMessageTypeEnum.Property2TenantOnRequestReceivedMessage,
                        new PartyInfo(_tenantRequest.PropertyId, _tenantRequest.PropertyNoReplyEmail),
                        new PartyInfo(_tenantRequest.TenantId,_tenantRequest.TenantEmail)
                    );

                case TenantRequestStatusEnum.Declined:
                    //"Hi [TenantFullName], /r/n Unfortunately, we cannot start your service request from [RequestDate]. Please call us [PropertyPhone] at your convenience. /r/n  Sincerely, /r/n [PropertyName]  "
                    //Title = "Your Request Declined",
                    return new PropertyMessage
                    (
                        PropertyMessageTypeEnum.Property2TenantOnRequestRejectedMessage,
                        new PartyInfo( _tenantRequest.PropertyId,_tenantRequest.PropertyNoReplyEmail),
                        new PartyInfo( _tenantRequest.TenantId, _tenantRequest.TenantEmail)
                    );

                case TenantRequestStatusEnum.Scheduled:
                    //" Hello [WorkerFullName], /r/n A new work order is scheduled for you. Please visit [WorkOrderPageUrl] for details. Sincerely, /r/n [PropertyName]"
                    //Title = ""You have new work order"",
                    return new PropertyMessage
                    (
                        PropertyMessageTypeEnum.Property2WorkerOnRequestScheduledMessage,
                        new PartyInfo(_tenantRequest.PropertyId,_tenantRequest.PropertyNoReplyEmail),
                        new PartyInfo(_worker.Id.ToString( ),_tenantRequest.WorkerEmail)
                        );
                case TenantRequestStatusEnum.Done:
                    //"Hello [SuperintendentFullName], /r/n I completed work order [WorkOrderNumber]. For report, please visit [WorkReportPageUrl].  Sincerely, /r/n [WorkerFullName]"
                    //Title = "Work order completed",

                    return new PropertyMessage
                    (
                        PropertyMessageTypeEnum.Worker2PropertyWorkDoneMessage,
                        new PartyInfo(_worker.Id.ToString(), _tenantRequest.PropertyNoReplyEmail),
                        new PartyInfo( _tenantRequest.PropertyId, _tenantRequest.SuperintendentEmail)
                    );
                case TenantRequestStatusEnum.Failed:
                    //"Hello [SuperintendentFullName], /r/n Unfortunately, the work order N[WorkOrderNumber] from [WorkOrderDate] can't be completed. For the report, please visit [WorkReportPageUrl].  Sincerely, /r/n [WorkerFullName]"
                    //Title = "Work can't be completed",

                    return new PropertyMessage
                    (
                        PropertyMessageTypeEnum.Worker2PropertyWorkIncompleteMessage,
                        new PartyInfo(_worker.Id.ToString(), _tenantRequest.PropertyNoReplyEmail),
                        new PartyInfo( _tenantRequest.PropertyId, _tenantRequest.SuperintendentEmail)
                    );

                case TenantRequestStatusEnum.Closed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_tenantRequest.RequestStatus));
            }


            return null;
        }
    }
}
