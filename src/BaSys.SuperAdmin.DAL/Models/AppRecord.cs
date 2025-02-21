using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BaSys.SuperAdmin.DAL.Models;

public class AppRecord
{
    public const int DefaultWorkflowPollInterval = 1000;

    private int _workflowPollInterval = DefaultWorkflowPollInterval;

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Memo { get; set; } = string.Empty;
    public bool UseWorkflowsScheduler { get; set; } = true;


    // Interval in ms.
    public int WorkflowPollInterval { 
        get { return _workflowPollInterval; } 
        set {
            if (value >= 100)
            {
                _workflowPollInterval = value;
            }
        } 
    }
    public List<DbInfoRecord> DbInfoRecords { get; set; } = new List<DbInfoRecord>();

    public AppRecord()
    {
    }

    public AppRecord(AppRecord source)
    {
        Fill(source);
    }

    public void Fill(AppRecord record)
    {
        Title = record.Title;
        Memo = record.Memo;
        UseWorkflowsScheduler = record.UseWorkflowsScheduler;
        WorkflowPollInterval = record.WorkflowPollInterval;
    }

    public override bool Equals(object? obj)
    {
        return obj is AppRecord description &&
               Id == description.Id;
    }

    public override int GetHashCode()
    {
        return 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);
    }

    public override string ToString()
    {
        return $"{Id}/{Title}";
    }
}