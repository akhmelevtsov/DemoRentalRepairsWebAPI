using RentalRepairs.Domain.Common;
using System.Text.RegularExpressions;

namespace RentalRepairs.Domain.ValueObjects;

public class PersonContactInfo : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public PersonContactInfo(string firstName, string lastName, string emailAddress, string? mobilePhone = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));
        
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email address cannot be empty", nameof(emailAddress));
        
        if (!EmailRegex.IsMatch(emailAddress))
            throw new ArgumentException("Invalid email address format", nameof(emailAddress));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        EmailAddress = emailAddress.Trim().ToLowerInvariant();
        MobilePhone = mobilePhone?.Trim();
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public string? MobilePhone { get; }

    public string GetFullName() => $"{FirstName} {LastName}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return EmailAddress;
        yield return MobilePhone ?? string.Empty;
    }

    public override string ToString() => $"{GetFullName()} ({EmailAddress})";
}