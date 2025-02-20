using BaSys.SuperAdmin.DAL.Models;

namespace BaSys.SuperAdmin.DTO;

public class AppRecordDto
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Memo { get; set; }
    public bool UseWorkflowsScheduler { get; set; }
    public int WorkflowThreadsCount { get; set; }
    public int MaxConcurrentWorkflows { get; set; }
    public int WorkflowPollInterval { get; set; }

    public AppRecordDto()
    {
    }

    public AppRecordDto(AppRecord model)
    {
        Id = model.Id;
        Title = model.Title;
        Memo = model.Memo;
        UseWorkflowsScheduler = model.UseWorkflowsScheduler;
        WorkflowThreadsCount = model.WorkflowThreadsCount;
        MaxConcurrentWorkflows = model.MaxConcurrentWorkflows;
        WorkflowPollInterval = model.WorkflowPollInterval;
    }

    public AppRecord ToModel()
    {
        return new AppRecord
        {
            Id = Id ?? string.Empty,
            Title = Title ?? string.Empty,
            Memo = Memo,
            UseWorkflowsScheduler = UseWorkflowsScheduler,
            WorkflowThreadsCount = WorkflowThreadsCount,
            MaxConcurrentWorkflows = MaxConcurrentWorkflows,
            WorkflowPollInterval = WorkflowPollInterval
        };
    }
}