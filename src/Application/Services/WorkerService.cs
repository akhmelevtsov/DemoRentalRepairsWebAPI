using MediatR;
using RentalRepairs.Application.Commands.Workers;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Interfaces;
using RentalRepairs.Application.Queries.Workers;

namespace RentalRepairs.Application.Services;

public class WorkerService : IWorkerService
{
    private readonly IMediator _mediator;

    public WorkerService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<int> RegisterWorkerAsync(WorkerDto workerDto, CancellationToken cancellationToken = default)
    {
        var command = new RegisterWorkerCommand
        {
            ContactInfo = workerDto.ContactInfo,
            Specialization = workerDto.Specialization
        };

        return await _mediator.Send(command, cancellationToken);
    }

    public async Task UpdateWorkerSpecializationAsync(int workerId, string specialization, CancellationToken cancellationToken = default)
    {
        var command = new UpdateWorkerSpecializationCommand
        {
            WorkerId = workerId,
            Specialization = specialization
        };

        await _mediator.Send(command, cancellationToken);
    }

    public async Task<WorkerDto> GetWorkerByIdAsync(int workerId, CancellationToken cancellationToken = default)
    {
        var query = new GetWorkerByIdQuery(workerId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<WorkerDto> GetWorkerByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var query = new GetWorkerByEmailQuery(email);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<WorkerDto>> GetWorkersAsync(string? specialization = null, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = new GetWorkersQuery
        {
            Specialization = specialization,
            IsActive = activeOnly
        };

        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<List<WorkerDto>> GetAvailableWorkersAsync(DateTime serviceDate, string? requiredSpecialization = null, CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableWorkersQuery(serviceDate)
        {
            RequiredSpecialization = requiredSpecialization
        };

        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<bool> IsWorkerAvailableAsync(string workerEmail, DateTime serviceDate, CancellationToken cancellationToken = default)
    {
        var availableWorkers = await GetAvailableWorkersAsync(serviceDate, cancellationToken: cancellationToken);
        return availableWorkers.Any(w => w.ContactInfo.EmailAddress.Equals(workerEmail, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<string>> GetWorkerSpecializationsAsync(CancellationToken cancellationToken = default)
    {
        var workers = await GetWorkersAsync(cancellationToken: cancellationToken);
        var specializations = workers
            .Where(w => !string.IsNullOrEmpty(w.Specialization))
            .Select(w => w.Specialization!)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        return specializations;
    }
}