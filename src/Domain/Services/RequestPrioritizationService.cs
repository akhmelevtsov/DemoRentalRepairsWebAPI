using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Domain.Exceptions;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Domain.Services;

public class RequestPrioritizationService
{
    private readonly ITenantRequestRepository _tenantRequestRepository;
    private readonly IPropertyRepository _propertyRepository;

    public RequestPrioritizationService(
        ITenantRequestRepository tenantRequestRepository,
        IPropertyRepository propertyRepository)
    {
        _tenantRequestRepository = tenantRequestRepository;
        _propertyRepository = propertyRepository;
    }

    public async Task<IEnumerable<TenantRequest>> GetPrioritizedRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var pendingSpec = new PendingTenantRequestsSpecification();
        var pendingRequests = await _tenantRequestRepository.GetBySpecificationAsync(pendingSpec, cancellationToken);

        // Calculate priority scores and sort
        var prioritizedRequests = pendingRequests
            .Select(request => new
            {
                Request = request,
                Priority = CalculatePriorityScore(request)
            })
            .OrderByDescending(x => x.Priority)
            .Select(x => x.Request);

        return prioritizedRequests;
    }

    public async Task<IEnumerable<TenantRequest>> GetUrgentRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var urgentSpec = new TenantRequestsByUrgencySpecification("Critical");
        var criticalRequests = await _tenantRequestRepository.GetBySpecificationAsync(urgentSpec, cancellationToken);

        var highSpec = new TenantRequestsByUrgencySpecification("High");
        var highRequests = await _tenantRequestRepository.GetBySpecificationAsync(highSpec, cancellationToken);

        var overdueSpec = new OverdueTenantRequestsSpecification(DateTime.UtcNow.AddDays(-2));
        var overdueRequests = await _tenantRequestRepository.GetBySpecificationAsync(overdueSpec, cancellationToken);

        return criticalRequests
            .Concat(highRequests)
            .Concat(overdueRequests)
            .Distinct()
            .OrderByDescending(CalculatePriorityScore);
    }

    public int CalculatePriorityScore(TenantRequest request)
    {
        var score = 0;

        // Base urgency score
        score += request.UrgencyLevel switch
        {
            "Critical" => 100,
            "High" => 75,
            "Normal" => 50,
            "Low" => 25,
            _ => 25
        };

        // Age factor - older requests get higher priority
        var ageInDays = (DateTime.UtcNow - request.CreatedAt).Days;
        score += Math.Min(ageInDays * 5, 50); // Max 50 points for age

        // Safety-related keywords get bonus points
        if (IsSafetyRelated(request))
        {
            score += 30;
        }

        // Emergency keywords get bonus points
        if (IsEmergency(request))
        {
            score += 50;
        }

        // Multiple service attempts get priority
        if (request.ServiceWorkOrderCount > 1)
        {
            score += request.ServiceWorkOrderCount * 10;
        }

        return score;
    }

    public bool IsSafetyRelated(TenantRequest request)
    {
        var text = $"{request.Title} {request.Description}".ToLowerInvariant();
        var safetyKeywords = new[]
        {
            "gas", "smoke", "fire", "electrical", "shock", "dangerous", "hazard",
            "carbon monoxide", "leak", "broken glass", "exposed wire", "safety"
        };

        return safetyKeywords.Any(keyword => text.Contains(keyword));
    }

    public bool IsEmergency(TenantRequest request)
    {
        var text = $"{request.Title} {request.Description}".ToLowerInvariant();
        var emergencyKeywords = new[]
        {
            "emergency", "urgent", "immediate", "flooding", "no heat", "no power",
            "broken door", "security", "lockout", "burst pipe", "severe"
        };

        return emergencyKeywords.Any(keyword => text.Contains(keyword));
    }

    public async Task<Dictionary<string, int>> GetRequestStatisticsByUrgencyAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var startDate = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = toDate ?? DateTime.UtcNow;

        var dateRangeSpec = new TenantRequestsByDateRangeSpecification(startDate, endDate);
        var requests = await _tenantRequestRepository.GetBySpecificationAsync(dateRangeSpec, cancellationToken);

        return requests
            .GroupBy(r => r.UrgencyLevel)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<TenantRequestStatus, int>> GetRequestStatisticsByStatusAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var startDate = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = toDate ?? DateTime.UtcNow;

        var dateRangeSpec = new TenantRequestsByDateRangeSpecification(startDate, endDate);
        var requests = await _tenantRequestRepository.GetBySpecificationAsync(dateRangeSpec, cancellationToken);

        return requests
            .GroupBy(r => r.Status)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<TenantRequest>> GetStaleRequestsAsync(
        int daysThreshold = 7,
        CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
        var overdueSpec = new OverdueTenantRequestsSpecification(thresholdDate);
        
        return await _tenantRequestRepository.GetBySpecificationAsync(overdueSpec, cancellationToken);
    }
}