﻿namespace BaSys.Host.DAL.Migrations.Base;

public abstract class MigrationBase
{
    public abstract Guid Uid { get; }
    public abstract DateTime MigrationUtcIdentifier { get; }
    public abstract string? Name { get; }
    public virtual string? Description { get; }
    public abstract Task Up();
    public abstract Task Down();
}