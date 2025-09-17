using MediatR;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Application.Commands.Properties.Handlers;

public class RegisterPropertyCommandHandler : ICommandHandler<RegisterPropertyCommand, int>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly PropertyDomainService _propertyDomainService;

    public RegisterPropertyCommandHandler(
        IPropertyRepository propertyRepository,
        PropertyDomainService propertyDomainService)
    {
        _propertyRepository = propertyRepository;
        _propertyDomainService = propertyDomainService;
    }

    public async Task<int> Handle(RegisterPropertyCommand request, CancellationToken cancellationToken)
    {
        // Validate business rules using domain service
        await _propertyDomainService.ValidatePropertyRegistrationAsync(request.Code, request.Units, cancellationToken);

        // Create value objects
        var address = new PropertyAddress(
            request.Address.StreetNumber,
            request.Address.StreetName,
            request.Address.City,
            request.Address.PostalCode);

        var superintendent = new PersonContactInfo(
            request.Superintendent.FirstName,
            request.Superintendent.LastName,
            request.Superintendent.EmailAddress,
            request.Superintendent.MobilePhone);

        // Create the property aggregate
        var property = new Property(
            request.Name,
            request.Code,
            address,
            request.PhoneNumber,
            superintendent,
            request.Units,
            request.NoReplyEmailAddress);

        // Save to repository
        await _propertyRepository.AddAsync(property, cancellationToken);
        await _propertyRepository.SaveChangesAsync(cancellationToken);

        return property.Id;
    }
}