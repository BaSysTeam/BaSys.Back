using BaSys.Common.Abstractions;

namespace BaSys.DAL.Models;

public class Migration: SystemObjectBase
{
    public Guid MigrationUid { get; set; }
    public string MigrationName { get; set; } = string.Empty;
    public DateTime ApplyDateTime { get; set; }

    public override string ToString()
    {
        return MigrationName;
    }
}