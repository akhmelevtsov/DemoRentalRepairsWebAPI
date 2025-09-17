using Mapster;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Application.Mappings;

/// <summary>
/// Mapster configuration for mapping between domain entities and application DTOs
/// </summary>
public static class DomainToResponseMappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Property, PropertyDto>
            .NewConfig()
            .Map(dest => dest.Tenants, src => src.Tenants);

        TypeAdapterConfig<PropertyAddress, PropertyAddressDto>
            .NewConfig()
            .Map(dest => dest.FullAddress, src => src.FullAddress);

        TypeAdapterConfig<PersonContactInfo, PersonContactInfoDto>
            .NewConfig()
            .Map(dest => dest.FullName, src => src.GetFullName());

        TypeAdapterConfig<Tenant, TenantDto>
            .NewConfig()
            .Map(dest => dest.PropertyCode, src => src.Property.Code)
            .Map(dest => dest.PropertyName, src => src.Property.Name)
            .Map(dest => dest.Requests, src => src.Requests);

        TypeAdapterConfig<TenantRequest, TenantRequestDto>
            .NewConfig()
            .Map(dest => dest.RequestChanges, src => src.RequestChanges);

        TypeAdapterConfig<TenantRequestChange, TenantRequestChangeDto>
            .NewConfig();

        TypeAdapterConfig<Worker, WorkerDto>
            .NewConfig();
    }
}