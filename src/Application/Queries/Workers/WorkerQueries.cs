using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Queries.Workers;

public class GetWorkerByIdQuery : IQuery<WorkerDto>
{
    public int WorkerId { get; set; }

    public GetWorkerByIdQuery(int workerId)
    {
        WorkerId = workerId;
    }
}

public class GetWorkerByEmailQuery : IQuery<WorkerDto>
{
    public string Email { get; set; } = default!;

    public GetWorkerByEmailQuery(string email)
    {
        Email = email;
    }
}

public class GetWorkersQuery : IQuery<List<WorkerDto>>
{
    public string? Specialization { get; set; }
    public bool? IsActive { get; set; }
}

public class GetAvailableWorkersQuery : IQuery<List<WorkerDto>>
{
    public DateTime ServiceDate { get; set; }
    public string? RequiredSpecialization { get; set; }

    public GetAvailableWorkersQuery(DateTime serviceDate)
    {
        ServiceDate = serviceDate;
    }
}