using RentalRepairs.Application.Common.Interfaces;

namespace RentalRepairs.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public string? UserId => "system"; // For now, default to system user
    public string? UserName => "System";
    public bool IsAuthenticated => true;
}