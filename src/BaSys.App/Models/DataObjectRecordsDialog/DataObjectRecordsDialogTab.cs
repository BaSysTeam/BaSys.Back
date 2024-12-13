using BaSys.DTO.Core;

namespace BaSys.App.Models.DataObjectRecordsDialog
{
    public sealed class DataObjectRecordsDialogTab
    {
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<DataTableColumnDto> Columns { get; set; } = new List<DataTableColumnDto>();

        public override string ToString()
        {
            return Title;
        }
    }
}
