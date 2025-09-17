using RentalRepairs.Domain.Common;

namespace RentalRepairs.Domain.ValueObjects;

public class ServiceWorkScheduleInfo : ValueObject
{
    public ServiceWorkScheduleInfo(
        DateTime serviceDate,
        string workerEmail,
        string workOrderNumber,
        int workOrderSequence)
    {
        if (serviceDate <= DateTime.UtcNow)
            throw new ArgumentException("Service date must be in the future", nameof(serviceDate));
        
        if (string.IsNullOrWhiteSpace(workerEmail))
            throw new ArgumentException("Worker email cannot be empty", nameof(workerEmail));
        
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            throw new ArgumentException("Work order number cannot be empty", nameof(workOrderNumber));

        ServiceDate = serviceDate;
        WorkerEmail = workerEmail;
        WorkOrderNumber = workOrderNumber;
        WorkOrderSequence = workOrderSequence;
    }

    public DateTime ServiceDate { get; }
    public string WorkerEmail { get; }
    public string WorkOrderNumber { get; }
    public int WorkOrderSequence { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ServiceDate;
        yield return WorkerEmail;
        yield return WorkOrderNumber;
        yield return WorkOrderSequence;
    }

    public override string ToString() => 
        $"Work Order {WorkOrderNumber} scheduled for {ServiceDate:yyyy-MM-dd} with {WorkerEmail}";
}