namespace RentalRepairs.Application.DTOs;

public class PropertyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public PropertyAddressDto Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public PersonContactInfoDto Superintendent { get; set; } = default!;
    public List<string> Units { get; set; } = new();
    public string NoReplyEmailAddress { get; set; } = default!; // Changed from NotificationEmail
    public DateTime CreatedDate { get; set; }
    public List<TenantDto> Tenants { get; set; } = new();
}

public class PropertyAddressDto
{
    public string StreetNumber { get; set; } = default!;
    public string StreetName { get; set; } = default!;
    public string City { get; set; } = default!;
    public string PostalCode { get; set; } = default!;
    public string FullAddress { get; set; } = default!;
}

public class PersonContactInfoDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string EmailAddress { get; set; } = default!;
    public string? MobilePhone { get; set; } // Changed from PhoneNumber
    public string FullName { get; set; } = default!;
}