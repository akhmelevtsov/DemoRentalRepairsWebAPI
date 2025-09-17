using FluentValidation;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Validators;
using RentalRepairs.Domain.Exceptions;

namespace RentalRepairs.Domain.Services;

public class DomainValidationService
{
    private readonly PropertyValidator _propertyValidator;
    private readonly TenantValidator _tenantValidator;
    private readonly TenantRequestValidator _tenantRequestValidator;
    private readonly WorkerValidator _workerValidator;

    public DomainValidationService()
    {
        _propertyValidator = new PropertyValidator();
        _tenantValidator = new TenantValidator();
        _tenantRequestValidator = new TenantRequestValidator();
        _workerValidator = new WorkerValidator();
    }

    public async Task ValidateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
    {
        var validationResult = entity switch
        {
            Property property => await _propertyValidator.ValidateAsync(property, cancellationToken),
            Tenant tenant => await _tenantValidator.ValidateAsync(tenant, cancellationToken),
            TenantRequest request => await _tenantRequestValidator.ValidateAsync(request, cancellationToken),
            Worker worker => await _workerValidator.ValidateAsync(worker, cancellationToken),
            _ => throw new ArgumentException($"No validator found for type {typeof(T).Name}")
        };

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainException($"Validation failed for {typeof(T).Name}: {errors}");
        }
    }

    public void Validate<T>(T entity) where T : class
    {
        ValidateAsync(entity).GetAwaiter().GetResult();
    }
}