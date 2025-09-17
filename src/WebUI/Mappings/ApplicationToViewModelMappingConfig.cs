using Mapster;
using RentalRepairs.Application.DTOs;
using RentalRepairs.WebUI.Models;

namespace RentalRepairs.WebUI.Mappings;

/// <summary>
/// Mapster configuration for mapping between Application DTOs and WebUI ViewModels
/// </summary>
public static class ApplicationToViewModelMappingConfig
{
    public static void RegisterMappings()
    {
        // Property mappings
        TypeAdapterConfig<PropertyDto, PropertyDetailsViewModel>
            .NewConfig()
            .Map(dest => dest.FullAddress, src => src.Address.FullAddress)
            .Map(dest => dest.SuperintendentName, src => src.Superintendent.FullName);

        TypeAdapterConfig<PropertyDto, PropertySummaryViewModel>
            .NewConfig()
            .Map(dest => dest.City, src => src.Address.City)
            .Map(dest => dest.State, src => src.Address.PostalCode) // Assuming PostalCode is used as State
            .Map(dest => dest.SuperintendentName, src => src.Superintendent.FullName);

        TypeAdapterConfig<RegisterPropertyViewModel, PropertyDto>
            .NewConfig();

        // Tenant Request mappings
        TypeAdapterConfig<TenantRequestDto, TenantRequestSummaryViewModel>
            .NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString());

        TypeAdapterConfig<TenantRequestDto, TenantRequestDetailsViewModel>
            .NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString());

        TypeAdapterConfig<SubmitTenantRequestViewModel, TenantRequestDto>
            .NewConfig();

        // Dashboard mappings - simplified to just copy basic properties
        TypeAdapterConfig<PropertyDto, PropertySummaryViewModel>
            .NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Code, src => src.Code)
            .Map(dest => dest.City, src => src.Address.City)
            .Map(dest => dest.SuperintendentName, src => src.Superintendent.FullName);
    }
}