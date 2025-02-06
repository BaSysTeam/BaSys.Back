using System.Data;
using BaSys.Common.Enums;
using BaSys.Host.DAL.Abstractions;
using BaSys.SuperAdmin.DAL.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace BaSys.Host.DAL;

public sealed class MainConnectionFactory : IMainConnectionFactory
{
    private readonly IDbInfoRecordsProvider _dbInfoRecordsProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MainConnectionFactory(IDbInfoRecordsProvider dbInfoRecordsProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbInfoRecordsProvider = dbInfoRecordsProvider;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public IDbConnection CreateConnection()
    {
        var dbName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
        if (string.IsNullOrEmpty(dbName))
            throw new NotImplementedException($"DbName is not set!");
        
        var dbInfoRecord = _dbInfoRecordsProvider.GetDbInfoRecordByDbName(dbName);
       
        IDbConnection connection;
        switch (dbInfoRecord?.DbKind)
        {
            case DbKinds.MsSql:
                connection = new SqlConnection(dbInfoRecord!.ConnectionString);
                break;
            case DbKinds.PgSql:
                connection = new NpgsqlConnection(dbInfoRecord!.ConnectionString);
                break;
            default:
                throw new NotImplementedException($"DbKind {dbInfoRecord?.DbKind} not supported");
        }
        
        return connection;
    }

    public IDbConnection CreateConnection(string connectionString, DbKinds dbKind)
    {
        IDbConnection connection = null;
        switch (dbKind)
        {
            case DbKinds.MsSql:
                connection = new SqlConnection(connectionString);
                break;
            case DbKinds.PgSql:
                connection = new NpgsqlConnection(connectionString);
                break;
            default:
                throw new NotImplementedException($"DbKind {dbKind} not supported");
        }

        return connection;
    }


}