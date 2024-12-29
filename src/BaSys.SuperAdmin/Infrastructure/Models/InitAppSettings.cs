using BaSys.Common.Enums;

namespace BaSys.SuperAdmin.Infrastructure.Models;

public class CurrentAppConfig
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
}

public class SuperAdminConfig
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DbKinds? DbKind { get; set; }
    public string ConnectionString { get; set; } = null!;
}

public class MainDbConfig
{
    public string Name { get; set; } = null!;
    public DbKinds? DbKind { get; set; }
    public string ConnectionString { get; set; } = null!;
    public string AdminLogin { get; set; } = null!;
    public string AdminPassword { get; set; } = null!;
    public string Culture { get; set; } = "en";
}

public class InitAppSettings
{
    public CurrentAppConfig? CurrentApp { get; set; }
    public SuperAdminConfig? Sa { get; set; }
    public MainDbConfig? MainDb { get; set; }
}