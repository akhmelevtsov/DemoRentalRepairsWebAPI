using RentalRepairs.Domain.Common;

namespace RentalRepairs.Domain.ValueObjects;

public class PropertyAddress : ValueObject
{
    public PropertyAddress(string streetNumber, string streetName, string city, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(streetNumber))
            throw new ArgumentException("Street number cannot be empty", nameof(streetNumber));
        
        if (string.IsNullOrWhiteSpace(streetName))
            throw new ArgumentException("Street name cannot be empty", nameof(streetName));
        
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));
        
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));

        StreetNumber = streetNumber;
        StreetName = streetName;
        City = city;
        PostalCode = postalCode;
    }

    public string StreetNumber { get; }
    public string StreetName { get; }
    public string City { get; }
    public string PostalCode { get; }

    public string FullAddress => $"{StreetNumber} {StreetName}, {City}, {PostalCode}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StreetNumber;
        yield return StreetName;
        yield return City;
        yield return PostalCode;
    }

    public override string ToString() => FullAddress;
}