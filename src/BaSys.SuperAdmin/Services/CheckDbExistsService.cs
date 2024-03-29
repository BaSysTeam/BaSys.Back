using BaSys.Common.Enums;
using BaSys.SuperAdmin.Abstractions;
using BaSys.SuperAdmin.DAL.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace BaSys.SuperAdmin.Services;

public class CheckDbExistsService : ICheckDbExistsService
{
    public async Task<bool?> IsExists(DbInfoRecord dbInfoRecord)
    {
        if (string.IsNullOrEmpty(dbInfoRecord.ConnectionString))
            return null;
        
        switch (dbInfoRecord.DbKind)
        {
            case DbKinds.MsSql:
                return await IsExistsMsSql(dbInfoRecord.ConnectionString);
            case DbKinds.PgSql:
                return await IsExistsPgSql(dbInfoRecord.ConnectionString);
            default:
                return null;
        }
    }

    #region private methods
    private async Task<bool?> IsExistsPgSql(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var dbName = builder.Database;
        builder.Remove("Database");
        builder.CommandTimeout = 1;
        builder.Timeout = 1;
        
        try
        {
            using var db = new NpgsqlConnection(builder.ConnectionString);
            var dbId = await db.QueryFirstOrDefaultAsync($"SELECT oid FROM pg_catalog.pg_database WHERE lower(datname) = lower('{dbName}');");
        
            if (dbId == null)
                return false;
            return true;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    private async Task<bool?> IsExistsMsSql(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        var dbName = builder.InitialCatalog;
        builder.Remove("Initial Catalog");
        builder.CommandTimeout = 1;
        builder.ConnectTimeout = 1;

        try
        {
            using var db = new SqlConnection(builder.ConnectionString);
            var dbId = await db.QueryFirstAsync<long?>($"SELECT DB_ID('{dbName}')");
        
            if (dbId == null)
                return false;
            return true;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    #endregion
}