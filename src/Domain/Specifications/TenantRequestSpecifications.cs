using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Domain.Specifications;

public class TenantRequestWithDetailsSpecification : BaseSpecification<TenantRequest>
{
    public TenantRequestWithDetailsSpecification() : base()
    {
        AddInclude(tr => tr.Tenant);
        AddInclude(tr => tr.RequestChanges);
        AddInclude("Tenant.Property");
    }
}

public class TenantRequestByStatusSpecification : BaseSpecification<TenantRequest>
{
    public TenantRequestByStatusSpecification(TenantRequestStatus status) 
        : base(tr => tr.Status == status)
    {
        AddInclude(tr => tr.Tenant);
        AddInclude("Tenant.Property");
        ApplyOrderByDescending(tr => tr.CreatedAt);
    }
}

public class TenantRequestByPropertySpecification : BaseSpecification<TenantRequest>
{
    public TenantRequestByPropertySpecification(int propertyId) 
        : base(tr => tr.Tenant.Property.Id == propertyId)
    {
        AddInclude(tr => tr.Tenant);
        AddInclude(tr => tr.RequestChanges);
        ApplyOrderByDescending(tr => tr.CreatedAt);
    }
}

public class TenantRequestByTenantSpecification : BaseSpecification<TenantRequest>
{
    public TenantRequestByTenantSpecification(int tenantId) 
        : base(tr => tr.Tenant.Id == tenantId)
    {
        AddInclude(tr => tr.RequestChanges);
        ApplyOrderByDescending(tr => tr.CreatedAt);
    }
}

public class PendingTenantRequestsSpecification : BaseSpecification<TenantRequest>
{
    public PendingTenantRequestsSpecification() 
        : base(tr => tr.Status == TenantRequestStatus.Submitted || tr.Status == TenantRequestStatus.Scheduled)
    {
        AddInclude(tr => tr.Tenant);
        AddInclude("Tenant.Property");
        ApplyOrderBy(tr => tr.CreatedAt);
    }
}

public class OverdueTenantRequestsSpecification : BaseSpecification<TenantRequest>
{
    public OverdueTenantRequestsSpecification(DateTime overdueDate) 
        : base(tr => tr.Status == TenantRequestStatus.Scheduled && tr.CreatedAt < overdueDate)
    {
        AddInclude(tr => tr.Tenant);
        AddInclude("Tenant.Property");
        ApplyOrderBy(tr => tr.CreatedAt);
    }
}

public class TenantRequestsByUrgencySpecification : BaseSpecification<TenantRequest>
{
    public TenantRequestsByUrgencySpecification(string urgencyLevel) 
        : base(tr => tr.UrgencyLevel == urgencyLevel && 
                     (tr.Status == TenantRequestStatus.Submitted || tr.Status == TenantRequestStatus.Scheduled))
    {
        AddInclude(tr => tr.Tenant);
        AddInclude("Tenant.Property");
        ApplyOrderBy(tr => tr.CreatedAt);
    }
}

public class TenantRequestsByDateRangeSpecification : BaseSpecification<TenantRequest>
{
    public TenantRequestsByDateRangeSpecification(DateTime startDate, DateTime endDate) 
        : base(tr => tr.CreatedAt >= startDate && tr.CreatedAt <= endDate)
    {
        AddInclude(tr => tr.Tenant);
        AddInclude("Tenant.Property");
        ApplyOrderByDescending(tr => tr.CreatedAt);
    }
}