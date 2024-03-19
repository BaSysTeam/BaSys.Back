using BaSys.Common.Enums;
using BaSys.Host.Infrastructure;

namespace BaSys.Host.Abstractions;

public interface IContextService
{
    ConnectionItem? GetConnectionItem(DbKinds? dbKind = null);
}