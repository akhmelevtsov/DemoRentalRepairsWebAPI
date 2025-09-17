using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Interfaces;

/// <summary>
/// Application service for worker management operations
/// </summary>
public interface IWorkerService
{
    // Worker Management
    Task<int> RegisterWorkerAsync(WorkerDto workerDto, CancellationToken cancellationToken = default);
    Task UpdateWorkerSpecializationAsync(int workerId, string specialization, CancellationToken cancellationToken = default);
    
    // Worker Retrieval
    Task<WorkerDto> GetWorkerByIdAsync(int workerId, CancellationToken cancellationToken = default);
    Task<WorkerDto> GetWorkerByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<WorkerDto>> GetWorkersAsync(string? specialization = null, bool activeOnly = true, CancellationToken cancellationToken = default);
    Task<List<WorkerDto>> GetAvailableWorkersAsync(DateTime serviceDate, string? requiredSpecialization = null, CancellationToken cancellationToken = default);
    
    // Worker Business Operations
    Task<bool> IsWorkerAvailableAsync(string workerEmail, DateTime serviceDate, CancellationToken cancellationToken = default);
    Task<List<string>> GetWorkerSpecializationsAsync(CancellationToken cancellationToken = default);
}