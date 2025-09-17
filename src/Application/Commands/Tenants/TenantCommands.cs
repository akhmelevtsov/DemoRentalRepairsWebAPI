using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Commands.Tenants;

public class RegisterTenantCommand : ICommand<int>
{
    public int PropertyId { get; set; }
    public PersonContactInfoDto ContactInfo { get; set; } = default!;
    public string UnitNumber { get; set; } = default!;
}