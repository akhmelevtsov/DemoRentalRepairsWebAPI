using System;
using System.Diagnostics;
using System.IO;
using mailslurp.Api;
using mailslurp.Client;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;

namespace Demo.RentalRepairs.Azure.Tests
{
    public class TestFixture : IDisposable
    {
        private readonly Process _funcHostProcess;

        //public readonly HttpClient Client = new HttpClient();

        public TestFixture()
        {

 
            var root = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var settings = new AppSettings();
            root.Bind(settings);

            var dotnetExePath = Environment.ExpandEnvironmentVariables(settings.DotNetExecutablePath);
            var functionHostPath = Environment.ExpandEnvironmentVariables(settings.FunctionHostPath);
            var functionAppFolder = Path.GetRelativePath(Directory.GetCurrentDirectory(), settings.FunctionApplicationPath);

            
            Config = new Configuration();
            Config.ApiKey.Add("x-api-key", settings.SlurpApiKey );

          
            ApiInstance = new InboxControllerApi(Config);


            _funcHostProcess = new Process
            {
                StartInfo =
                {
                    FileName = dotnetExePath,
                    Arguments = $"\"{functionHostPath}\" start -p {Port}",
                    WorkingDirectory = functionAppFolder
                }
            };
            var success = _funcHostProcess.Start();
            if (!success)
            {
                throw new InvalidOperationException("Could not start Azure Functions host.");
            }

            var storageAccount = CloudStorageAccount.Parse(settings.StorageConnectionString);


           
            TableClient = storageAccount.CreateCloudTableClient();

            foreach (var table in TableClient.ListTables())
                table.DeleteIfExists();

        }

        public int Port { get; } = 7071;

        public CloudTableClient TableClient { get; }

        public InboxControllerApi ApiInstance { get; }

        public Configuration Config { get; }

        public virtual void Dispose()
        {
            if (!_funcHostProcess.HasExited)
            {
                _funcHostProcess.Kill();
            }

            _funcHostProcess.Dispose();
        }
    }
}
