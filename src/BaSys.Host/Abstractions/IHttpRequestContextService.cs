using BaSys.Common.Enums;
using BaSys.Host.Infrastructure;

namespace BaSys.Host.Abstractions;

public interface IHttpRequestContextService
{
    DbKinds? GetConnectionKind();
    ConnectionItem? GetConnectionItem(DbKinds? dbKind = null);
}