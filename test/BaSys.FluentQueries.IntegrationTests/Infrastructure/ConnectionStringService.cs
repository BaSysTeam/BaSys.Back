using BaSys.SuperAdmin.DAL.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.FluentQueries.IntegrationTests.Infrastructure
{
    internal class ConnectionStringService
    {
        public const string PgConnectionStringName = "pg_test_base";
        public const string MsConnectionStringName = "ms_test_base";

        private readonly IConfigurationRoot _configuration;
        private readonly Dictionary<string, DbInfoRecord> _connectionStrings = new Dictionary<string, DbInfoRecord>();

        public string PgConnectionString => _connectionStrings[PgConnectionStringName]?.ConnectionString ?? string.Empty;
        public string MsConnectionString => _connectionStrings[MsConnectionStringName]?.ConnectionString ?? string.Empty;

        public ConnectionStringService()
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _configuration = builder.Build();

            var pgConnectionString = _configuration.GetConnectionString(PgConnectionStringName);    
            var msConnectionString = _configuration.GetConnectionString(MsConnectionStringName);

            var pgDbInfoRecord = new DbInfoRecord()
            {
                ConnectionString = pgConnectionString,
                DbKind = Common.Enums.DbKinds.PgSql
            };

            var msDbInfoRecord = new DbInfoRecord()
            {
                ConnectionString = msConnectionString,
                DbKind = Common.Enums.DbKinds.MsSql
            };

            _connectionStrings.Add(PgConnectionStringName, pgDbInfoRecord);
            _connectionStrings.Add(MsConnectionStringName, msDbInfoRecord);
        }

        public string GetConnectionString(string connectionStringName)
        {
            return _connectionStrings[connectionStringName]?.ConnectionString ?? string.Empty   ;
        }

        public DbInfoRecord GetDbInfoRecord(string connectionStringName)
        {
            return _connectionStrings[connectionStringName];
        }
    }
}
