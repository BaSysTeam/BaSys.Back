using BaSys.Metadata.Models;

namespace BaSys.Host.DAL.Abstractions
{
    public interface IMetaObjectChangeColumnCommand: IMetaObjectChangeCommand
    {
        MetaObjectTableColumn Column { get; set; }
    }
}
