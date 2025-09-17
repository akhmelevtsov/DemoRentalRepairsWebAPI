namespace RentalRepairs.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class PropertyDomainException : DomainException
{
    public PropertyDomainException(string message) : base(message)
    {
    }

    public PropertyDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class TenantRequestDomainException : DomainException
{
    public TenantRequestDomainException(string message) : base(message)
    {
    }

    public TenantRequestDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class WorkerDomainException : DomainException
{
    public WorkerDomainException(string message) : base(message)
    {
    }

    public WorkerDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}