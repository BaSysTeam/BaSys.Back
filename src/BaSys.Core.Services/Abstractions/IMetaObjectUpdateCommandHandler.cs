using BaSys.DTO.Metadata;

namespace BaSys.Core.Features.Abstractions
{
    public interface IMetaObjectUpdateCommandHandler: ICommandHandlerBase<MetaObjectStorableSettingsDto, int>
    {
    }
}
