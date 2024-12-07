namespace BaSys.App.Models.DataObjectRecordsDialog
{
    public sealed class DataObjectRecordsDialogTab
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;

        public override string ToString()
        {
            return Title;
        }
    }
}
