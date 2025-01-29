namespace BaSys.Workflows.DTO
{
    public sealed class WorkflowStartDto
    {
        public string Name { get; set; } = string.Empty;
        public List<WorkflowParameterDto> Parameters { get; set; } = new List<WorkflowParameterDto>();
    }
}
