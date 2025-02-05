using BaSys.Common.Enums;
using BaSys.Logging.Abstractions;
using BaSys.Logging.Workflow.Abstractions;
using Dapper;
using Npgsql;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace BaSys.Logging.Workflow
{
    public sealed class WorkflowPgSqlLogger : WorkflowLogger
    {

        public WorkflowPgSqlLogger(LoggerConfig loggerConfig, WorkflowLoggerContext context) : base(loggerConfig, context)
        {
            if (string.IsNullOrEmpty(loggerConfig.ConnectionString) || string.IsNullOrEmpty(loggerConfig.WorkflowsLogTableName))
                return;

            CheckDbExists(loggerConfig.ConnectionString);

            var columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            {"raise_date", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},

            {"log_message", new SinglePropertyColumnWriter("LogMessage", PropertyWriteMethod.Raw, NpgsqlDbType.Text)},
            {"kind", new SinglePropertyColumnWriter("Kind", PropertyWriteMethod.Raw, NpgsqlDbType.Integer)},
            {"level", new SinglePropertyColumnWriter("Level", PropertyWriteMethod.Raw, NpgsqlDbType.Integer)},

            {"origin", new SinglePropertyColumnWriter("Origin", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)},

            {"workflow_name", new SinglePropertyColumnWriter("WorkflowName", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)},
            {"workflow_uid", new SinglePropertyColumnWriter("WorkflowUid", PropertyWriteMethod.Raw, NpgsqlDbType.Uuid)},
            {"run_uid", new SinglePropertyColumnWriter("RunUid", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)},
            {"version", new SinglePropertyColumnWriter("Version", PropertyWriteMethod.Raw, NpgsqlDbType.Bigint)},

            {"step_name", new SinglePropertyColumnWriter("StepName", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)},

            {"user_uid", new SinglePropertyColumnWriter("UserUid", PropertyWriteMethod.Raw, NpgsqlDbType.Varchar)},
            {"user_name", new SinglePropertyColumnWriter("UserName")},

        };

            try
            {
                Logger = new LoggerConfiguration()
                    .WriteTo
                    .PostgreSQL(connectionString: loggerConfig.ConnectionString,
                        tableName: loggerConfig.WorkflowsLogTableName,
                        columnOptions: columnWriters,
                        needAutoCreateTable: true)
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WorkflowPgSqlLogger. Cannot init logger: {ex.Message}, {ex.StackTrace}");
            }

        }

        protected override void WriteInner(WorkflowLogEventKinds kind, EventTypeLevels level, string stepName, string message)
        {
            Logger?.Information("{LogMessage} {Kind} {Level} {Version} {Origin} {WorkflowName} {WorkflowUid} {RunUid} {UserUid} {UserName} {StepName}",
           message,
           (int)kind,
           (int)level,
           _version,
           _origin,
           _workflowName,
           _workflowUid,
           _runUid,
           _userUid,
           _userName,
           stepName);
        }

        private void CheckDbExists(string connectionString)
        {
            try
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);

                var dbName = builder.Database;
                builder.Remove("Database");
                builder.CommandTimeout = 1;
                builder.Timeout = 1;

                using var db = new NpgsqlConnection(builder.ConnectionString);
                var dbId = db.QueryFirstOrDefault($"SELECT oid FROM pg_catalog.pg_database WHERE lower(datname) = lower('{dbName}');");

                if (dbId != null)
                    return;

                // create db
                var sql = @$"CREATE DATABASE {dbName}
                    WITH
                        OWNER = {builder.Username}
                        ENCODING = 'UTF8'
                        LOCALE_PROVIDER = 'libc'
                        CONNECTION LIMIT = -1
                        IS_TEMPLATE = False;";
                db.Execute(sql);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WorkflowPgSqlLogger. Cannot create DB: {ex.Message}, {ex.StackTrace}");
            }
        }


    }
}
