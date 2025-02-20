using Microsoft.EntityFrameworkCore.Query.Internal;

namespace BaSys.SuperAdmin.DAL.Models;

public class AppRecord
{
    public const int DefaultWorkflowThreadsCount = 1;
    public const int DefaultMaxConcurrentWorkflows = 100;
    public const int DefaultWorkflowPollInterval = 1000;

    private int _workflowThreadsCount = DefaultWorkflowThreadsCount;
    private int _maxCuncurrentWorkflows = DefaultMaxConcurrentWorkflows;
    private int _workflowPollInterval = DefaultWorkflowPollInterval;

    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Memo { get; set; } = string.Empty;
    public bool UseWorkflowsScheduler { get; set; } = true;

    // Not used with in-memory
    public int WorkflowThreadsCount
    {
        get { return _workflowThreadsCount; }
        set
        {

            if (value >= 1)
            {
                _workflowThreadsCount = value;
            }

        }
    }

    // Not used with in-memory
    public int MaxConcurrentWorkflows
    {
        get { return _maxCuncurrentWorkflows; }
        set
        {
            if (value >= 1)
            {
                _maxCuncurrentWorkflows = value;
            }
        }
    }

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
        WorkflowThreadsCount = record.WorkflowThreadsCount;
        MaxConcurrentWorkflows = record.MaxConcurrentWorkflows;
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