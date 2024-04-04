using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.IntegrationTests
{
    [TestFixture]
    public class CreateTableBuilderTests
    {
        private IConfigurationRoot _configuration;

        [SetUp]
        public void Setup()
        {
            // Build configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();
        }

        [TestCase("pg_test_base")]
        [TestCase("ms_test_base")]
        public void GetConnectionStrings(string connectionStringName)
        {
            // Retrieve the connection string
            string connectionString = _configuration?.GetConnectionString("pg_test_base") ?? string.Empty;

            Console.WriteLine($"{connectionStringName}:{connectionString}");

            Assert.IsNotNull(connectionString);
        }
    }
}
