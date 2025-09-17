using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Domain.Specifications;

public class PropertyWithTenantsSpecification : BaseSpecification<Property>
{
    public PropertyWithTenantsSpecification() : base()
    {
        AddInclude(p => p.Tenants);
    }
}

public class PropertyByCodeSpecification : BaseSpecification<Property>
{
    public PropertyByCodeSpecification(string code) 
        : base(p => p.Code == code)
    {
    }
}

public class PropertiesWithUnitsSpecification : BaseSpecification<Property>
{
    public PropertiesWithUnitsSpecification(List<string> unitNumbers) 
        : base(p => p.Units.Any(u => unitNumbers.Contains(u)))
    {
    }
}

public class PropertiesByCitySpecification : BaseSpecification<Property>
{
    public PropertiesByCitySpecification(string city) 
        : base(p => p.Address.City == city)
    {
        ApplyOrderBy(p => p.Name);
    }
}

public class PropertiesBySuperintendentEmailSpecification : BaseSpecification<Property>
{
    public PropertiesBySuperintendentEmailSpecification(string email) 
        : base(p => p.Superintendent.EmailAddress == email)
    {
        AddInclude(p => p.Tenants);
    }
}