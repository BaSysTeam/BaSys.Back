using BaSys.Common.Abstractions;
using BaSys.DTO.Metadata;

namespace BaSys.Core.Features.Abstractions
{
    public interface IMetaObjectCreateCommandHandler: ICommandHandlerBase<MetaObjectStorableSettingsDto, int>
    {
    }
}
