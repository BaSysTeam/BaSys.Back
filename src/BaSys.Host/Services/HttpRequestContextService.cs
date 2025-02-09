﻿using System.Security.Claims;
using BaSys.Common.Enums;
using BaSys.Host.Abstractions;
using BaSys.Host.Infrastructure;
using BaSys.Host.Infrastructure.Abstractions;

namespace BaSys.Host.Services;

public class HttpRequestContextService : IHttpRequestContextService
{
    private readonly IDataSourceProvider _dataSourceProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpRequestContextService(IDataSourceProvider dataSourceProvider, IHttpContextAccessor httpContextAccessor)
    {
        _dataSourceProvider = dataSourceProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public DbKinds? GetConnectionKind()
    {
        return GetConnectionItem()?.DbKind;
    }

    public ConnectionItem? GetConnectionItem(DbKinds? dbKind = null)
    {
        ConnectionItem? item;

        // if init work db
        if (IsInitDbRequest(_httpContextAccessor.HttpContext?.Request.RouteValues))
        {
            var value = _httpContextAccessor.HttpContext?.Request.RouteValues["id"];
            int.TryParse(value?.ToString(), out var dbInfoId);
            item = _dataSourceProvider.GetConnectionItemByDbInfoId(dbInfoId);
            return item;
        }

        // auth from auth endpoint
        if (_httpContextAccessor.HttpContext?.Request.Path == "/api/public/v1/auth" &&  //  "/api/auth"
            _httpContextAccessor.HttpContext?.Request.Query.TryGetValue("dbid", out var val) == true)
        {
            var dbName = val.FirstOrDefault();
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentException("DbId not set in auth query!");

            item = _dataSourceProvider.GetConnectionItemByDbName(dbName);
            return item;
        }

        // From user
        var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var userDbName = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "DbName")?.Value;
        if (!string.IsNullOrEmpty(userId))
            return _dataSourceProvider.GetCurrentConnectionItemByUser(userId, userDbName);

        // auth from Login form
        if (_httpContextAccessor.HttpContext?.Request.HasFormContentType == true &&
            _httpContextAccessor.HttpContext?.Request.ContentType != null &&
            _httpContextAccessor.HttpContext?.Request.Form?.TryGetValue("Input.DbName", out var requestDbName) == true)
        {
            item = _dataSourceProvider.GetConnectionItemByDbName(requestDbName);
            if (item == null)
            {
                throw new ArgumentNullException($"Db \"{requestDbName}\" not found.");
            }
            return item;
        }

        item = _dataSourceProvider.GetDefaultConnectionItem(dbKind);

        return item;
    }

    private bool IsInitDbRequest(RouteValueDictionary? requestRouteValues)
    {
        if (requestRouteValues?.Any() != true)
            return false;

        if (!requestRouteValues.Any(x => x.Key == "action" && x.Value?.ToString() == "InitDb"))
            return false;

        if (!requestRouteValues.Any(x => x.Key == "controller" && x.Value?.ToString() == "WorkDb"))
            return false;

        if (!requestRouteValues.Any(x => x.Key == "id"))
            return false;

        return true;
    }
}