namespace BaSys.DAL.Models;

public class Migration
{
    public Guid Uid { get; set; }
    public Guid MigrationUid { get; set; }
    public string MigrationName { get; set; }
    public DateTime ApplyDateTime { get; set; }
}