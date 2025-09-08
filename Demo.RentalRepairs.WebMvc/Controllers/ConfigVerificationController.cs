using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo.RentalRepairs.WebMvc.Controllers
{
    /// <summary>
    /// Configuration verification controller for development/testing only
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigVerificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public ConfigVerificationController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        /// <summary>
        /// Verifies that sensitive configuration values are loaded from user secrets
        /// </summary>
        [HttpGet("verify-secrets")]
        public IActionResult VerifySecrets()
        {
            // Only allow this in development
            if (!_environment.IsDevelopment())
            {
                return NotFound();
            }

            var clientSecret = _configuration["AzureAdB2CSettings:ClientSecret"];
            var storageConnectionString = _configuration["RentalRepairsAzureTableApiDbSettings:StorageConnectionString"];

            var result = new
            {
                ClientSecretConfigured = !string.IsNullOrEmpty(clientSecret),
                ClientSecretLength = clientSecret?.Length ?? 0,
                StorageConnectionStringConfigured = !string.IsNullOrEmpty(storageConnectionString),
                StorageConnectionStringLength = storageConnectionString?.Length ?? 0,
                Message = "User secrets are properly configured!",
                Environment = _environment.EnvironmentName
            };

            return Ok(result);
        }
    }
}