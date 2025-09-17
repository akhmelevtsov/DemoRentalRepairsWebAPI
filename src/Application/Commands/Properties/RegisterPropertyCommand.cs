using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;

namespace RentalRepairs.Application.Commands.Properties;

public class RegisterPropertyCommand : ICommand<int>
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public PropertyAddressDto Address { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public PersonContactInfoDto Superintendent { get; set; } = default!;
    public List<string> Units { get; set; } = new();
    public string NoReplyEmailAddress { get; set; } = default!; // Changed from NotificationEmail
}