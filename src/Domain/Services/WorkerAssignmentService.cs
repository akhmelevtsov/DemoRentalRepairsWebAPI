using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Specifications;
using RentalRepairs.Domain.Exceptions;
using RentalRepairs.Domain.Enums;

namespace RentalRepairs.Domain.Services;

public class WorkerAssignmentService
{
    private readonly IWorkerRepository _workerRepository;
    private readonly ITenantRequestRepository _tenantRequestRepository;

    public WorkerAssignmentService(
        IWorkerRepository workerRepository,
        ITenantRequestRepository tenantRequestRepository)
    {
        _workerRepository = workerRepository;
        _tenantRequestRepository = tenantRequestRepository;
    }

    public async Task<Worker> FindBestWorkerForRequestAsync(
        TenantRequest request, 
        DateTime serviceDate,
        CancellationToken cancellationToken = default)
    {
        // Get active workers
        var activeWorkersSpec = new ActiveWorkersSpecification();
        var activeWorkers = await _workerRepository.GetBySpecificationAsync(activeWorkersSpec, cancellationToken);

        if (!activeWorkers.Any())
        {
            throw new WorkerDomainException("No active workers available for assignment");
        }

        // Try to find a worker with matching specialization based on request type
        var preferredSpecialization = DetermineRequiredSpecialization(request);
        if (!string.IsNullOrEmpty(preferredSpecialization))
        {
            var specializedWorkersSpec = new WorkerBySpecializationSpecification(preferredSpecialization);
            var specializedWorkers = await _workerRepository.GetBySpecificationAsync(specializedWorkersSpec, cancellationToken);
            
            if (specializedWorkers.Any())
            {
                // Find the least busy specialized worker
                return await FindLeastBusyWorkerAsync(specializedWorkers, serviceDate, cancellationToken);
            }
        }

        // If no specialized workers, find the least busy general worker
        return await FindLeastBusyWorkerAsync(activeWorkers, serviceDate, cancellationToken);
    }

    public async Task<bool> IsWorkerAvailableAsync(
        Worker worker, 
        DateTime serviceDate,
        CancellationToken cancellationToken = default)
    {
        if (!worker.IsActive)
        {
            return false;
        }

        // Check if worker has any scheduled requests on the same date
        var startOfDay = serviceDate.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var dateRangeSpec = new TenantRequestsByDateRangeSpecification(startOfDay, endOfDay);
        var requestsOnDate = await _tenantRequestRepository.GetBySpecificationAsync(dateRangeSpec, cancellationToken);

        // Count how many scheduled requests this worker has on this date
        var workerRequestsCount = requestsOnDate.Count(r => 
            r.Status == TenantRequestStatus.Scheduled && 
            IsRequestAssignedToWorker(r, worker.ContactInfo.EmailAddress));

        // Assuming a worker can handle up to 3 requests per day
        return workerRequestsCount < 3;
    }

    public async Task<IEnumerable<Worker>> GetAvailableWorkersAsync(
        DateTime serviceDate,
        string? requiredSpecialization = null,
        CancellationToken cancellationToken = default)
    {
        var availableWorkers = new List<Worker>();

        IEnumerable<Worker> workers;
        if (!string.IsNullOrEmpty(requiredSpecialization))
        {
            var spec = new WorkerBySpecializationSpecification(requiredSpecialization);
            workers = await _workerRepository.GetBySpecificationAsync(spec, cancellationToken);
        }
        else
        {
            var spec = new ActiveWorkersSpecification();
            workers = await _workerRepository.GetBySpecificationAsync(spec, cancellationToken);
        }

        foreach (var worker in workers)
        {
            if (await IsWorkerAvailableAsync(worker, serviceDate, cancellationToken))
            {
                availableWorkers.Add(worker);
            }
        }

        return availableWorkers;
    }

    public string DetermineRequiredSpecialization(TenantRequest request)
    {
        var title = request.Title.ToLowerInvariant();
        var description = request.Description.ToLowerInvariant();

        // Simple keyword-based specialization determination
        if (ContainsKeywords(title, description, "plumb", "leak", "water", "drain", "pipe"))
            return "Plumbing";

        if (ContainsKeywords(title, description, "electric", "power", "outlet", "wiring", "light"))
            return "Electrical";

        if (ContainsKeywords(title, description, "heat", "hvac", "air", "furnace", "thermostat"))
            return "HVAC";

        if (ContainsKeywords(title, description, "paint", "wall", "floor", "door", "window", "repair"))
            return "General Maintenance";

        return "General Maintenance"; // Default
    }

    private static bool ContainsKeywords(string title, string description, params string[] keywords)
    {
        var text = $"{title} {description}";
        return keywords.Any(keyword => text.Contains(keyword));
    }

    private async Task<Worker> FindLeastBusyWorkerAsync(
        IEnumerable<Worker> workers, 
        DateTime serviceDate,
        CancellationToken cancellationToken)
    {
        var workerWorkload = new Dictionary<Worker, int>();

        foreach (var worker in workers)
        {
            // Count upcoming scheduled requests for this worker
            var upcomingRequestsCount = await GetWorkerUpcomingWorkloadAsync(worker, serviceDate, cancellationToken);
            workerWorkload[worker] = upcomingRequestsCount;
        }

        // Return the worker with the least workload
        return workerWorkload.OrderBy(w => w.Value).First().Key;
    }

    private async Task<int> GetWorkerUpcomingWorkloadAsync(
        Worker worker, 
        DateTime fromDate,
        CancellationToken cancellationToken)
    {
        var endDate = fromDate.AddDays(7); // Next 7 days
        var dateRangeSpec = new TenantRequestsByDateRangeSpecification(fromDate, endDate);
        var upcomingRequests = await _tenantRequestRepository.GetBySpecificationAsync(dateRangeSpec, cancellationToken);

        return upcomingRequests.Count(r => 
            r.Status == TenantRequestStatus.Scheduled && 
            IsRequestAssignedToWorker(r, worker.ContactInfo.EmailAddress));
    }

    private static bool IsRequestAssignedToWorker(TenantRequest request, string workerEmail)
    {
        // This would need to check the request changes for scheduled work with this worker
        // For now, simplified implementation
        return request.RequestChanges.Any(change => 
            change.Status == TenantRequestStatus.Scheduled && 
            change.Description.Contains(workerEmail));
    }
}