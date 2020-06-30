namespace Demo.RentalRepairs.Domain.Framework
{
    public interface IUserRoleProvider<TUserRoleEnum>
    {
        TUserRoleEnum UserRole { get; set;  }
    }
}
