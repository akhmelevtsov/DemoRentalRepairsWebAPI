using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Queries.Tenants;

public class GetTenantByIdQuery : IQuery<TenantDto>
{
    public int TenantId { get; set; }

    public GetTenantByIdQuery(int tenantId)
    {
        TenantId = tenantId;
    }
}

public class GetTenantByPropertyAndUnitQuery : IQuery<TenantDto>
{
    public int PropertyId { get; set; }
    public string UnitNumber { get; set; } = default!;

    public GetTenantByPropertyAndUnitQuery(int propertyId, string unitNumber)
    {
        PropertyId = propertyId;
        UnitNumber = unitNumber;
    }
}

public class GetTenantsByPropertyQuery : IQuery<List<TenantDto>>
{
    public int PropertyId { get; set; }
    public bool WithActiveRequestsOnly { get; set; }

    public GetTenantsByPropertyQuery(int propertyId)
    {
        PropertyId = propertyId;
    }
}