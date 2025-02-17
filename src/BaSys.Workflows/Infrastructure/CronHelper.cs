using Cronos;

namespace BaSys.Workflows.Infrastructure
{
    public static class CronHelper
    {
        public static bool CronMatches(string cronExpression, DateTime now)
        {
            var cron = CronExpression.Parse(cronExpression, CronFormat.Standard);
            var previousOccurrence = cron.GetNextOccurrence(now.AddMinutes(-1), TimeZoneInfo.Utc);

            return previousOccurrence.HasValue && previousOccurrence.Value.Minute == now.Minute;
        }
    }
}
