using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Commands.Workers;

public class RegisterWorkerCommand : ICommand<int>
{
    public PersonContactInfoDto ContactInfo { get; set; } = default!;
    public string? Specialization { get; set; }
}

public class UpdateWorkerSpecializationCommand : ICommand
{
    public int WorkerId { get; set; }
    public string Specialization { get; set; } = default!;
}