using BaSys.Common.Infrastructure;

namespace BaSys.Host.Abstractions
{
    public interface IMegaMenuService
    {
        ResultWrapper<List<MegaMenuItemDto>> GetItems();
    }

    public class MegaMenuItemDto
    {
        public Guid Key { get; set; } 
        public required string Label { get; set; }
        public bool Visible { get; set; } = true;
        public List<List<ColumnItemDto>> Items { get; set; } = new List<List<ColumnItemDto>>();
    }

    public class ColumnItemDto
    {
        public Guid Key { get; set; }
        public required string Label { get; set; }
        public bool Visible { get; set; } = true;
        public List<ColumnItemPointDto> Items { get; set; } = new List<ColumnItemPointDto>();
    }

    public class ColumnItemPointDto
    {
        public Guid Key { get; set; }
        public required string Label { get; set; }
        public bool Visible { get; set; } = true;
    }
}
