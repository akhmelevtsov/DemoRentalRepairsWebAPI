using Demo.RentalRepairs.Domain.Entities;
using Demo.RentalRepairs.Domain.ValueObjects.Request;

namespace Demo.RentalRepairs.WebApi.Models
{
    public class TenantRequestModel
    {
        public TenantRequestStatusEnum RequestStatus { get; set;  }
        public TenantRequestDoc RequestDoc { get; set; }
        public TenantRequestRejectNotes RejectNotes { get; set; }
        public ServiceWorkReport WorkReport { get; set; }
        public ServiceWorkOrder ServiceWorkOrder { get; set; }
    }
}
