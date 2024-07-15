using BaSys.Common.Enums;
using BaSys.Metadata.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Metadata.DTOs
{
    public sealed class MetaObjectStorableSettingsDto
    {
        public string MetaObjectKindTitle { get; set; } = string.Empty;

        public string Uid { get; set; } = string.Empty;
        public Guid MetaObjectKindUid { get; set; }
        public EditMethods EditMethod { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public string OrderByExpression { get; set; } = string.Empty;
        public string DisplayExpression { get; set; } = string.Empty;
        public long Version { get; set; }
        public bool IsActive { get; set; }

        public MetaObjectTable Header { get; set; } = new MetaObjectTable();
        public List<MetaObjectTable> TableParts { get; set; } = new();

        public MetaObjectStorableSettingsDto()
        {

        }

        public MetaObjectStorableSettingsDto(MetaObjectStorableSettings settings, MetaObjectKindSettings kindSettings)
        {
            Uid = settings.Uid.ToString();
            EditMethod = settings.EditMethod;
            Title = settings.Title;
            Name = settings.Name;
            Memo = settings.Memo;
            OrderByExpression = settings.OrderByExpression;
            DisplayExpression = settings.DisplayExpression;
            IsActive = settings.IsActive;

            MetaObjectKindUid = kindSettings.Uid;
            MetaObjectKindTitle = kindSettings.Title;

            Header = settings.Header;
            TableParts = settings.TableParts;

        }

        public MetaObjectStorableSettings ToModel()
        {
            var model = new MetaObjectStorableSettings()
            {
                Uid = Guid.Parse(Uid),
                EditMethod = EditMethod,
                Title = Title,
                Name = Name,
                Memo = Memo,
                OrderByExpression = OrderByExpression,
                DisplayExpression = DisplayExpression,
                IsActive = IsActive,
                MetaObjectKindUid = MetaObjectKindUid,
                Header = Header,
                TableParts = TableParts,
            };

            return model;
        }
    }
}
