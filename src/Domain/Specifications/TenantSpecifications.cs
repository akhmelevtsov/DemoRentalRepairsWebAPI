using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Domain.Specifications;

public class TenantWithRequestsSpecification : BaseSpecification<Tenant>
{
    public TenantWithRequestsSpecification() : base()
    {
        AddInclude(t => t.Requests);
        AddInclude(t => t.Property);
    }
}

public class TenantByPropertySpecification : BaseSpecification<Tenant>
{
    public TenantByPropertySpecification(int propertyId) 
        : base(t => t.Property.Id == propertyId)
    {
        ApplyOrderBy(t => t.UnitNumber);
    }
}

public class TenantByEmailSpecification : BaseSpecification<Tenant>
{
    public TenantByEmailSpecification(string email) 
        : base(t => t.ContactInfo.EmailAddress == email)
    {
        AddInclude(t => t.Property);
        AddInclude(t => t.Requests);
    }
}

public class TenantByPropertyAndUnitSpecification : BaseSpecification<Tenant>
{
    public TenantByPropertyAndUnitSpecification(string propertyCode, string unitNumber) 
        : base(t => t.PropertyCode == propertyCode && t.UnitNumber == unitNumber)
    {
        AddInclude(t => t.Property);
        AddInclude(t => t.Requests);
    }
}

public class TenantsWithActiveRequestsSpecification : BaseSpecification<Tenant>
{
    public TenantsWithActiveRequestsSpecification() 
        : base(t => t.Requests.Any(r => r.Status == Enums.TenantRequestStatus.Submitted || 
                                        r.Status == Enums.TenantRequestStatus.Scheduled))
    {
        AddInclude(t => t.Property);
        AddInclude(t => t.Requests);
        ApplyOrderBy(t => t.Property.Name);
    }
}