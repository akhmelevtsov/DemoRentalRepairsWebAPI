namespace RentalRepairs.Application.DTOs;

public class TenantDto
{
    public int Id { get; set; }
    public string UnitNumber { get; set; } = default!;
    public PersonContactInfoDto ContactInfo { get; set; } = default!;
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = default!;
    public string PropertyName { get; set; } = default!;
    public DateTime RegistrationDate { get; set; }
    public List<TenantRequestDto> Requests { get; set; } = new();
}

public class WorkerDto
{
    public int Id { get; set; }
    public PersonContactInfoDto ContactInfo { get; set; } = default!;
    public string? Specialization { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public DateTime RegistrationDate { get; set; }
}