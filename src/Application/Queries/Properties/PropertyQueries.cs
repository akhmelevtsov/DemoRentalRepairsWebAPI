using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.Common.Models;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Queries.Properties;

public class GetPropertyByIdQuery : IQuery<PropertyDto>
{
    public int PropertyId { get; set; }

    public GetPropertyByIdQuery(int propertyId)
    {
        PropertyId = propertyId;
    }
}

public class GetPropertyByCodeQuery : IQuery<PropertyDto>
{
    public string Code { get; set; }

    public GetPropertyByCodeQuery(string code)
    {
        Code = code;
    }
}

public class GetPropertiesQuery : IQuery<PagedResult<PropertyDto>>
{
    public string? City { get; set; }
    public string? SuperintendentEmail { get; set; }
    public bool? WithTenants { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetPropertyStatisticsQuery : IQuery<PropertyStatisticsDto>
{
    public int PropertyId { get; set; }

    public GetPropertyStatisticsQuery(int propertyId)
    {
        PropertyId = propertyId;
    }
}

public class PropertyStatisticsDto
{
    public string PropertyName { get; set; } = default!;
    public string PropertyCode { get; set; } = default!;
    public int TotalUnits { get; set; }
    public int OccupiedUnits { get; set; }
    public int AvailableUnits { get; set; }
    public double OccupancyRate { get; set; }
    public string SuperintendentName { get; set; } = default!;
    public string SuperintendentEmail { get; set; } = default!;
}