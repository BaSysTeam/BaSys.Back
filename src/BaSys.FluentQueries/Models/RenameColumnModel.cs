namespace BaSys.FluentQueries.Models
{
    public class RenameColumnModel
    {
        public string OldName { get; set; } = string.Empty;
        public string NewName { get; set; } = string.Empty;

        public RenameColumnModel()
        {
            
        }

        public RenameColumnModel(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        public override string ToString()
        {
            return $"{OldName}->{NewName}";
        }
    }
}
