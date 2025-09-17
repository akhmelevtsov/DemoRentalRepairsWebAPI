using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.ValueObjects;

namespace RentalRepairs.Domain.Tests;

public class QuickValidation
{
    public static void TestBasicEntityCreation()
    {
        // Test PersonContactInfo
        var contact = new PersonContactInfo("John", "Doe", "john@example.com");
        
        // Test PropertyAddress  
        var address = new PropertyAddress("123", "Main St", "City", "12345");
        
        // Test Property creation
        var property = new Property(
            "Test Property", 
            "TP001", 
            address, 
            "555-1234", 
            contact, 
            new List<string> { "101", "102" }, 
            "noreply@test.com");
            
        // Test Tenant creation
        var tenant = property.RegisterTenant(contact, "101");
        
        // Test TenantRequest creation
        var request = tenant.CreateRequest("Test Issue", "Description", "Normal");
        
        Console.WriteLine("All domain entities created successfully!");
    }
}