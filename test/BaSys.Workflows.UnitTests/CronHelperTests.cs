using BaSys.Workflows.Infrastructure;

namespace BaSys.Workflows.UnitTests
{
    [TestFixture]
    public class CronHelperTests
    {
        [TestCase("* * * * *", "2025-02-17T21:15:45", true)]
        [TestCase("*/2 * * * *", "2025-02-17T21:15:45", false)]
        [TestCase("*/2 * * * *", "2025-02-17T21:16:03", true)]
        [TestCase("5 17 * * *", "2025-02-17T21:16:03", false)]
        [TestCase("5 17 * * *", "2025-02-17T17:05:03", true)]
        public void CronMatches_CronExpression_MatchResult(string cronExpression, string dateTimeIso, bool check)
        {
            var dateTime = DateTime.SpecifyKind(DateTime.Parse(dateTimeIso), DateTimeKind.Utc);
            var result = CronHelper.CronMatches(cronExpression, dateTime);

            Console.WriteLine($"Cron: {cronExpression}, date: {dateTime}, matches: {result}");

            Assert.That(result, Is.EqualTo(check));
        }
    }
}
