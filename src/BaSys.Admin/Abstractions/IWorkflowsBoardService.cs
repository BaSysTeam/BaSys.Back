namespace BaSys.Admin.Abstractions
{
    public interface IWorkflowsBoardService
    {
        Task<IEnumerable<string?>> GetInfoAsync();
    }
}
