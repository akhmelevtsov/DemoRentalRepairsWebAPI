using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Demo.RentalRepairs.WebMvc.Authorization
{
    public class WorkerExistsAuthorizationHandler : AuthorizationHandler<WorkerExistsRequirement>
    {
        private readonly IPropertyService _propertyService;

        public WorkerExistsAuthorizationHandler(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            WorkerExistsRequirement requirement)
        {
            var userEmail = context.User.Identity?.Name;
            
            if (string.IsNullOrEmpty(userEmail))
            {
                return Task.CompletedTask;
            }

            try
            {
                var worker = _propertyService.GetWorkerByEmail(userEmail);
                if (worker != null)
                {
                    context.Succeed(requirement);
                }
            }
            catch
            {
                // If worker doesn't exist or any error occurs, requirement fails
            }

            return Task.CompletedTask;
        }
    }
}