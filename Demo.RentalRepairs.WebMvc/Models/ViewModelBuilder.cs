using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.Enums;
using Demo.RentalRepairs.Domain.ValueObjects.Request;
using FluentValidation.Internal;

namespace Demo.RentalRepairs.WebMvc.Models
{
    public static class ViewModelBuilder
    {
        public static List<TenantRequestChangeViewModel> BuildViewModel(this List<TenantRequestChange> requestChanges)
        {
            var list = new List<TenantRequestChangeViewModel>();
            foreach (var e in requestChanges)
            {
                list.Add(new TenantRequestChangeViewModel()
                {
                    UpdateDate = e.DateCreated,
                    TenantRequestStatus = e.TenantRequestStatus.ToString().SplitPascalCase(),
                    Comments = e.Comments()
                });
            }

            return list.OrderByDescending( x => x.UpdateDate ).ToList() ;
        }

       

        public static PropertyTenantRequestViewModel BuildViewModel(this TenantRequest tenantRequest)
        {
            return new PropertyTenantRequestViewModel()
            {

                RequestCode = tenantRequest.Code,
                RequestStatus = tenantRequest.RequestStatus.ToString().SplitPascalCase(),
                DateCreated = tenantRequest.DateCreated,
                UnitNumber = tenantRequest.TenantUnit,
                RequestTitle = tenantRequest.RequestTitle,
                RequestDescription = tenantRequest.RequestDescription,
              
                ShowCloseButton = tenantRequest.RequestStatus == TenantRequestStatusEnum.Declined
                                  || tenantRequest.RequestStatus == TenantRequestStatusEnum.Done,
                ShowScheduleRejectButtons = tenantRequest.RequestStatus == TenantRequestStatusEnum.Submitted,
                ShowScheduleButton = tenantRequest.RequestStatus == TenantRequestStatusEnum.Failed ,
                ScheduledDate = DateTime.Now.Date,
                ShowReportButton = tenantRequest.RequestStatus == TenantRequestStatusEnum.Scheduled,
                PropertyCode = tenantRequest.Tenant.PropertyCode ,
                History = tenantRequest.RequestChanges.BuildViewModel()  
                
            };

        }

    


    }
}
