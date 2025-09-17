using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Enums;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace RentalRepairs.Domain.Services;

public interface IBusinessRule<in T>
{
    string RuleName { get; }
    Task<bool> IsSatisfiedAsync(T entity, CancellationToken cancellationToken = default);
    string GetViolationMessage(T entity);
}

public abstract class BusinessRuleBase<T> : IBusinessRule<T>
{
    public abstract string RuleName { get; }
    public abstract Task<bool> IsSatisfiedAsync(T entity, CancellationToken cancellationToken = default);
    public abstract string GetViolationMessage(T entity);
}

// Property Business Rules
public class PropertyCodeUniquenessRule : BusinessRuleBase<Property>
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyCodeUniquenessRule(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public override string RuleName => "Property Code Must Be Unique";

    public override async Task<bool> IsSatisfiedAsync(Property property, CancellationToken cancellationToken = default)
    {
        return !await _propertyRepository.ExistsAsync(property.Code, cancellationToken);
    }

    public override string GetViolationMessage(Property property)
    {
        return $"Property code '{property.Code}' already exists in the system";
    }
}

public class PropertyMustHaveUnitsRule : BusinessRuleBase<Property>
{
    public override string RuleName => "Property Must Have At Least One Unit";

    public override Task<bool> IsSatisfiedAsync(Property property, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(property.Units.Any());
    }

    public override string GetViolationMessage(Property property)
    {
        return "Property must have at least one unit defined";
    }
}

// Tenant Request Business Rules
public class TenantRequestStatusTransitionRule : BusinessRuleBase<(TenantRequest Request, TenantRequestStatus NewStatus)>
{
    private readonly Dictionary<TenantRequestStatus, List<TenantRequestStatus>> _allowedTransitions = new()
    {
        [TenantRequestStatus.Draft] = new() { TenantRequestStatus.Submitted },
        [TenantRequestStatus.Submitted] = new() { TenantRequestStatus.Scheduled, TenantRequestStatus.Declined },
        [TenantRequestStatus.Scheduled] = new() { TenantRequestStatus.Done, TenantRequestStatus.Failed },
        [TenantRequestStatus.Failed] = new() { TenantRequestStatus.Scheduled },
        [TenantRequestStatus.Done] = new() { TenantRequestStatus.Closed },
        [TenantRequestStatus.Declined] = new() { TenantRequestStatus.Closed }
    };

    public override string RuleName => "Tenant Request Status Transition Must Be Valid";

    public override Task<bool> IsSatisfiedAsync((TenantRequest Request, TenantRequestStatus NewStatus) input, CancellationToken cancellationToken = default)
    {
        var (request, newStatus) = input;
        
        if (!_allowedTransitions.ContainsKey(request.Status))
            return Task.FromResult(false);

        return Task.FromResult(_allowedTransitions[request.Status].Contains(newStatus));
    }

    public override string GetViolationMessage((TenantRequest Request, TenantRequestStatus NewStatus) input)
    {
        var (request, newStatus) = input;
        return $"Cannot transition request from {request.Status} to {newStatus}";
    }
}

public class WorkerMustBeActiveForSchedulingRule : BusinessRuleBase<(TenantRequest Request, Worker Worker)>
{
    public override string RuleName => "Worker Must Be Active For Scheduling";

    public override Task<bool> IsSatisfiedAsync((TenantRequest Request, Worker Worker) input, CancellationToken cancellationToken = default)
    {
        var (_, worker) = input;
        return Task.FromResult(worker.IsActive);
    }

    public override string GetViolationMessage((TenantRequest Request, Worker Worker) input)
    {
        var (_, worker) = input;
        return $"Worker '{worker.ContactInfo.GetFullName()}' is not active and cannot be assigned to requests";
    }
}

// Business Rules Engine
public class BusinessRulesEngine
{
    private readonly IServiceProvider _serviceProvider;

    public BusinessRulesEngine(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task ValidateAsync<T>(T entity, CancellationToken cancellationToken = default)
    {
        var rules = GetBusinessRules<T>();
        var violations = new List<string>();

        foreach (var rule in rules)
        {
            if (!await rule.IsSatisfiedAsync(entity, cancellationToken))
            {
                violations.Add($"{rule.RuleName}: {rule.GetViolationMessage(entity)}");
            }
        }

        if (violations.Any())
        {
            throw new DomainException($"Business rule violations: {string.Join("; ", violations)}");
        }
    }

    private IEnumerable<IBusinessRule<T>> GetBusinessRules<T>()
    {
        // In a real implementation, this would use dependency injection
        // to get all registered business rules for type T
        var rules = new List<IBusinessRule<T>>();

        if (typeof(T) == typeof(Property))
        {
            var propertyRepository = _serviceProvider.GetService<IPropertyRepository>();
            if (propertyRepository != null)
            {
                rules.Add((IBusinessRule<T>)new PropertyCodeUniquenessRule(propertyRepository));
                rules.Add((IBusinessRule<T>)new PropertyMustHaveUnitsRule());
            }
        }

        return rules;
    }
}