using MediatR;
using RentalRepairs.Application.Commands.Tenants;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Exceptions;

namespace RentalRepairs.Application.Commands.Tenants.Handlers;

public class RegisterTenantCommandHandler : ICommandHandler<RegisterTenantCommand, int>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly PropertyDomainService _propertyDomainService;

    public RegisterTenantCommandHandler(
        IPropertyRepository propertyRepository,
        ITenantRepository tenantRepository,
        PropertyDomainService propertyDomainService)
    {
        _propertyRepository = propertyRepository;
        _tenantRepository = tenantRepository;
        _propertyDomainService = propertyDomainService;
    }

    public async Task<int> Handle(RegisterTenantCommand request, CancellationToken cancellationToken)
    {
        // Validate tenant registration business rules
        await _propertyDomainService.ValidateTenantRegistrationAsync(
            request.PropertyId, 
            request.UnitNumber, 
            cancellationToken);

        // Get the property
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property == null)
        {
            throw new PropertyDomainException($"Property with ID '{request.PropertyId}' not found");
        }

        // Create contact info value object
        var contactInfo = new PersonContactInfo(
            request.ContactInfo.FirstName,
            request.ContactInfo.LastName,
            request.ContactInfo.EmailAddress,
            request.ContactInfo.MobilePhone);

        // Register tenant through property aggregate
        var tenant = await _propertyDomainService.RegisterTenantAsync(
            request.PropertyId,
            contactInfo,
            request.UnitNumber,
            cancellationToken);

        // Save the tenant
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _tenantRepository.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}