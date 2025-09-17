using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;
using RentalRepairs.Application.Queries.Workers;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.Specifications;
using Mapster;

namespace RentalRepairs.Application.Queries.Workers.Handlers;

public class GetWorkerByIdQueryHandler : IQueryHandler<GetWorkerByIdQuery, WorkerDto>
{
    private readonly IWorkerRepository _workerRepository;

    public GetWorkerByIdQueryHandler(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<WorkerDto> Handle(GetWorkerByIdQuery request, CancellationToken cancellationToken)
    {
        var worker = await _workerRepository.GetByIdAsync(request.WorkerId, cancellationToken);
        
        if (worker == null)
        {
            throw new ArgumentException($"Worker with ID '{request.WorkerId}' not found");
        }

        return worker.Adapt<WorkerDto>();
    }
}

public class GetWorkerByEmailQueryHandler : IQueryHandler<GetWorkerByEmailQuery, WorkerDto>
{
    private readonly IWorkerRepository _workerRepository;

    public GetWorkerByEmailQueryHandler(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<WorkerDto> Handle(GetWorkerByEmailQuery request, CancellationToken cancellationToken)
    {
        var worker = await _workerRepository.GetByEmailAsync(request.Email, cancellationToken);
        
        if (worker == null)
        {
            throw new ArgumentException($"Worker with email '{request.Email}' not found");
        }

        return worker.Adapt<WorkerDto>();
    }
}

public class GetWorkersQueryHandler : IQueryHandler<GetWorkersQuery, List<WorkerDto>>
{
    private readonly IWorkerRepository _workerRepository;

    public GetWorkersQueryHandler(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<List<WorkerDto>> Handle(GetWorkersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.Worker> workers;

        if (!string.IsNullOrEmpty(request.Specialization))
        {
            var specializationSpec = new WorkerBySpecializationSpecification(request.Specialization);
            workers = await _workerRepository.GetBySpecificationAsync(specializationSpec, cancellationToken);
        }
        else if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
            {
                var activeSpec = new ActiveWorkersSpecification();
                workers = await _workerRepository.GetBySpecificationAsync(activeSpec, cancellationToken);
            }
            else
            {
                workers = await _workerRepository.GetAllAsync(cancellationToken);
                workers = workers.Where(w => !w.IsActive);
            }
        }
        else
        {
            workers = await _workerRepository.GetAllAsync(cancellationToken);
        }

        return workers.Adapt<List<WorkerDto>>();
    }
}

public class GetAvailableWorkersQueryHandler : IQueryHandler<GetAvailableWorkersQuery, List<WorkerDto>>
{
    private readonly IWorkerRepository _workerRepository;

    public GetAvailableWorkersQueryHandler(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<List<WorkerDto>> Handle(GetAvailableWorkersQuery request, CancellationToken cancellationToken)
    {
        // For now, implement a simple version without WorkerAssignmentService
        // This can be enhanced later when the service is properly implemented
        var allWorkers = await _workerRepository.GetActiveWorkersAsync(cancellationToken);
        
        // Filter by specialization if required
        if (!string.IsNullOrEmpty(request.RequiredSpecialization))
        {
            allWorkers = allWorkers.Where(w => w.Specialization == request.RequiredSpecialization);
        }

        return allWorkers.Adapt<List<WorkerDto>>();
    }
}