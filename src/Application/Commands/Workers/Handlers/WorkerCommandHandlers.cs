using MediatR;
using RentalRepairs.Application.Commands.Workers;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.ValueObjects;
using RentalRepairs.Domain.Exceptions;

namespace RentalRepairs.Application.Commands.Workers.Handlers;

public class RegisterWorkerCommandHandler : ICommandHandler<RegisterWorkerCommand, int>
{
    private readonly IWorkerRepository _workerRepository;

    public RegisterWorkerCommandHandler(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task<int> Handle(RegisterWorkerCommand request, CancellationToken cancellationToken)
    {
        // Check if worker with same email already exists
        var existingWorker = await _workerRepository.GetByEmailAsync(request.ContactInfo.EmailAddress, cancellationToken);
        if (existingWorker != null)
        {
            throw new WorkerDomainException($"Worker with email '{request.ContactInfo.EmailAddress}' already exists");
        }

        // Create contact info value object
        var contactInfo = new PersonContactInfo(
            request.ContactInfo.FirstName,
            request.ContactInfo.LastName,
            request.ContactInfo.EmailAddress,
            request.ContactInfo.MobilePhone);

        // Create worker entity
        var worker = new Worker(contactInfo);

        // Set specialization if provided
        if (!string.IsNullOrEmpty(request.Specialization))
        {
            worker.SetSpecialization(request.Specialization);
        }

        // Save to repository
        await _workerRepository.AddAsync(worker, cancellationToken);
        await _workerRepository.SaveChangesAsync(cancellationToken);

        return worker.Id;
    }
}

public class UpdateWorkerSpecializationCommandHandler : ICommandHandler<UpdateWorkerSpecializationCommand>
{
    private readonly IWorkerRepository _workerRepository;

    public UpdateWorkerSpecializationCommandHandler(IWorkerRepository workerRepository)
    {
        _workerRepository = workerRepository;
    }

    public async Task Handle(UpdateWorkerSpecializationCommand request, CancellationToken cancellationToken)
    {
        var worker = await _workerRepository.GetByIdAsync(request.WorkerId, cancellationToken);
        if (worker == null)
        {
            throw new WorkerDomainException($"Worker with ID '{request.WorkerId}' not found");
        }

        // Update specialization
        worker.SetSpecialization(request.Specialization);

        await _workerRepository.SaveChangesAsync(cancellationToken);
    }
}