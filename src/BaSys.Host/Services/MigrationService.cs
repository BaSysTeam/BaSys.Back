using System.Text;
using BaSys.Host.Abstractions;
using BaSys.Host.DAL.Migrations.Base;

namespace BaSys.Host.Services;

public class MigrationService : IMigrationService
{
    public List<Migration>? GetMigrations()
    {
        var migrations = typeof(Migration)
            .Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Migration)) && !t.IsAbstract)
            .Select(t => (Migration)Activator.CreateInstance(t)!)
            .OrderBy(x => x.MigrationUtcIdentifier)
            .ToList();

        CheckMigrations(migrations);

        return migrations;
    }

    private void CheckMigrations(List<Migration> migrations)
    {
        // Same uids check
        var uidGroups = migrations.GroupBy(x => x.Uid);
        var sameUids = uidGroups.Where(x => x.Count() > 1).ToArray();
        if (sameUids.Any())
        {
            var sb = new StringBuilder();
            foreach (var sameUid in sameUids)
            {
                sb.Append($"{sameUid.Key},");
            }

            var str = sb.ToString().TrimEnd(',');
            throw new ArgumentException($"Duplicate migration uids: {str}");
        }
        
        // Same dates check
        var dateGroups = migrations.GroupBy(x => x.MigrationUtcIdentifier);
        var sameDates = dateGroups.Where(x => x.Count() > 1).ToArray();
        if (sameDates.Any())
        {
            var sb = new StringBuilder();
            foreach (var sameDate in sameDates)
            {
                sb.Append($"{sameDate.Key},");
            }

            var str = sb.ToString().TrimEnd(',');
            throw new ArgumentException($"Duplicate MigrationUtcIdentifier: {str}");
        }
        
        // Empty name check
        var emptyNameMigration = migrations.FirstOrDefault(x => string.IsNullOrEmpty(x.Name));
        if (emptyNameMigration != null)
            throw new ArgumentException($"Migration with uid: {emptyNameMigration.Uid} has no name!");
    }
}