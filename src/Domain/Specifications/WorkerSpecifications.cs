using RentalRepairs.Domain.Entities;

namespace RentalRepairs.Domain.Specifications;

public class ActiveWorkersSpecification : BaseSpecification<Worker>
{
    public ActiveWorkersSpecification() : base(w => w.IsActive)
    {
        ApplyOrderBy(w => w.ContactInfo.LastName);
    }
}

public class WorkerByEmailSpecification : BaseSpecification<Worker>
{
    public WorkerByEmailSpecification(string email) 
        : base(w => w.ContactInfo.EmailAddress == email)
    {
    }
}

public class WorkerBySpecializationSpecification : BaseSpecification<Worker>
{
    public WorkerBySpecializationSpecification(string specialization) 
        : base(w => w.IsActive && w.Specialization == specialization)
    {
        ApplyOrderBy(w => w.ContactInfo.LastName);
    }
}

public class WorkersAvailableForSchedulingSpecification : BaseSpecification<Worker>
{
    public WorkersAvailableForSchedulingSpecification() 
        : base(w => w.IsActive)
    {
        ApplyOrderBy(w => w.ContactInfo.LastName);
    }
}