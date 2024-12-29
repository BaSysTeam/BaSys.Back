using BaSys.Metadata.Helpers;
using BaSys.Translation;
using System;
using System.Collections.Generic;

namespace BaSys.Metadata.Models
{
    public static class MetaObjectKindDefaults
    {
        private const string RecordsRegisterUid = "{3EABC5D2-9CF5-4A74-95C8-FF2C2015DED3}";
        private static readonly RecordsSettingsColumns RecordsColumnsIdentifiers = new RecordsSettingsColumns()
        {
            PeriodColumnUid = "{8893E1DF-525E-4A3F-B421-988294B4A2A8}",
            ObjectKindColumnUid = "{DCB06A92-5D6F-46BE-AF10-88A4F91F47E6}",
            MetaObjectColumnUid = "{7C2B8AD7-034D-495B-BA80-E300EDC50146}",
            ObjectColumnUid = "{78C493F2-E58A-4CE6-8665-27AF40000562}",
            RowColumnUid = "{398C66AD-10CE-4C75-9A7B-F2BA9398972F}"
        };

        public static readonly MetaObjectKindSettings Menu = new MetaObjectKindSettings()
        {
            Uid = Guid.Parse("{4FB2CC15-2579-4D01-AEDB-F097430756F7}"),
            Name = "menu",
            Title = DictMain.Menu,
            Prefix = "mnu",
            IconClass = "pi pi-list",
            IsStandard = true
        };

        public static readonly MetaObjectKindSettings Catalog = BuildDefaultCatalogKindSettings();
        public static readonly MetaObjectKindSettings Enum = BuildDefaultEnumKindSettings();
        public static readonly MetaObjectKindSettings Register = BuildDefaultRegisterKindSettings();
        public static readonly MetaObjectKindSettings RecordsRegister = BuildDefaultRegisterRecordsKindSettings();
        public static readonly MetaObjectKindSettings Operation = BuildDefaultOperationKindSettings();


        public static IEnumerable<MetaObjectKindSettings> AllItems()
        {
            var collection = new List<MetaObjectKindSettings>
            {
                Menu,
                Catalog,
                Enum,
                Register,
                RecordsRegister,
                Operation
            };

            return collection;
        }

        private static MetaObjectKindSettings BuildDefaultCatalogKindSettings()
        {
            var settings = new MetaObjectKindSettings()
            {
                Uid = Guid.Parse("{032D8377-500F-4631-B435-1F7F69046674}"),
                Name = "catalog",
                Prefix = "cat",
                Title = DictMain.Catalog,
                IconClass = "pi pi-book",
                StoreData = true,
                IsReference = true,
                AllowAttacheFiles = true,
                OrderByExpression = "title",
                DisplayExpression = "title",
            };

            var idColumn = new MetaObjectKindStandardColumn(Guid.Parse("{4A7C739C-6012-4E45-BFC0-B78A06DB8AEC}"),
                                                            "id",
                                                            "Id",
                                                            new MetaObjectTableColumnDataSettings(dataTypeUid: DataTypeDefaults.Int.Uid,
                                                                                                  primaryKey: true));

            var titleColumn = new MetaObjectKindStandardColumn(Guid.Parse("{E163C0C4-DE77-4E58-A468-F3527B573E4A}"),
                                                          "title",
                                                           DictMain.Title,
                                                           new MetaObjectTableColumnDataSettings(dataTypeUid: DataTypeDefaults.String.Uid,
                                                                                                 stringLength: 100,
                                                                                                 required: true));

            settings.StandardColumns.Add(idColumn);
            settings.StandardColumns.Add(titleColumn);

            return settings;
        }

        private static MetaObjectKindSettings BuildDefaultEnumKindSettings()
        {
            var settings = new MetaObjectKindSettings()
            {
                Uid = Guid.Parse("{5449A8AD-9359-41B2-8FF2-3F415EB4DA76}"),
                Name = "enum",
                Prefix = "enm",
                Title = DictMain.Enum,
                IconClass = "pi pi-list",
                StoreData = true,
                IsReference = true,
                AllowAttacheFiles = false,
                OrderByExpression = "code",
                DisplayExpression = "title",
            };

            var nameColumn = new MetaObjectKindStandardColumn("{3CCEEBAD-7D5A-4943-A102-6267E22227A8}",
                                                              "name",
                                                              DictMain.Name,
                                                              DataSettingsBuilder.Make()
                                                                                 .DataType(DataTypeDefaults.String)
                                                                                 .StringLength(20)
                                                                                 .PrimaryKey(true)
                                                                                 .Build());



            var codeColumn = new MetaObjectKindStandardColumn("{E647FB57-470E-4547-9A94-2BE0F3D0985E}",
                                                              "code",
                                                              DictMain.Code,
                                                              DataSettingsBuilder.Make()
                                                                                 .DataType(DataTypeDefaults.String)
                                                                                 .StringLength(3)
                                                                                 .Required(true)
                                                                                 .Unique(true)
                                                                                 .Build());

            var titleColumn = new MetaObjectKindStandardColumn("{91A26AE4-5723-4109-8322-7E4E59129239}",
                                                               "title",
                                                               DictMain.Title,
                                                               DataSettingsBuilder.Make().DataType(DataTypeDefaults.String).StringLength(100).Required(true).Build());


            settings.StandardColumns.Add(nameColumn);
            settings.StandardColumns.Add(codeColumn);
            settings.StandardColumns.Add(titleColumn);

            return settings;
        }

        private static MetaObjectKindSettings BuildDefaultRegisterKindSettings()
        {
            var settings = new MetaObjectKindSettings()
            {
                Uid = Guid.Parse("{F3ECB444-8E72-4387-83F1-66C712DE1C32}"),
                Name = "register",
                Prefix = "reg",
                Title = DictMain.Register,
                IconClass = "pi pi-table",
                StoreData = true,
                IsReference = false,
                OrderByExpression = "id",
            };

            var idColumn = new MetaObjectKindStandardColumn(Guid.Parse("{96B5F50B-98DE-43FB-AEA5-70FC29A35763}"),
                                                            "id",
                                                            "Id",
                                                            DataSettingsBuilder.Make().DataType(DataTypeDefaults.Long).PrimaryKey(true).Build());

            settings.StandardColumns.Add(idColumn);

            return settings;
        }

        private static MetaObjectKindSettings BuildDefaultRegisterRecordsKindSettings()
        {
            var settings = new MetaObjectKindSettings()
            {
                Uid = Guid.Parse(RecordsRegisterUid),
                Name = "records",
                Prefix = "regr",
                Title = DictMain.RecordsRegister,
                IconClass = "pi pi-table",
                StoreData = true,
                IsReference = false,
                OrderByExpression = "id",
            };

            var idColumn = new MetaObjectKindStandardColumn(Guid.Parse("{7F7EB95C-7B70-4E4F-A391-4080D0710589}"),
                                                            "id",
                                                            "Id",
                                                            DataSettingsBuilder.Make().DataType(DataTypeDefaults.Long).PrimaryKey(true).Build());

            var periodColumn = new MetaObjectKindStandardColumn(Guid.Parse(RecordsColumnsIdentifiers.PeriodColumnUid),
                                                                "period",
                                                                DictMain.Period,
                                                                DataSettingsBuilder.Make().DataType(DataTypeDefaults.DateTime).Required(true).Build());

            var objectKindColumn = new MetaObjectKindStandardColumn(Guid.Parse(RecordsColumnsIdentifiers.ObjectKindColumnUid),
                                                                    "object_kind",
                                                                    DictMain.ObjectKind,
                                                                    DataSettingsBuilder.Make().DataType(DataTypeDefaults.MetaObjectKind).Required(true).Build());

            var metaObjectColumn = new MetaObjectKindStandardColumn(Guid.Parse(RecordsColumnsIdentifiers.MetaObjectColumnUid),
                                                                    "meta_object",
                                                                    DictMain.MetaObject,
                                                                    DataSettingsBuilder.Make().DataType(DataTypeDefaults.MetaObject).Required(true).Build());

            var objectColumn = new MetaObjectKindStandardColumn(Guid.Parse(RecordsColumnsIdentifiers.ObjectColumnUid),
                                                                "object_uid",
                                                                DictMain.Object,
                                                                DataSettingsBuilder.Make().DataType(DataTypeDefaults.Int).Required(true).Build());

            var rowNumberColumn = new MetaObjectKindStandardColumn(Guid.Parse(RecordsColumnsIdentifiers.RowColumnUid),
                                                               "row",
                                                               DictMain.RowNumber,
                                                               DataSettingsBuilder.Make().DataType(DataTypeDefaults.Int).Required(true).Build());


            settings.StandardColumns.Add(idColumn);
            settings.StandardColumns.Add(periodColumn);
            settings.StandardColumns.Add(objectKindColumn);
            settings.StandardColumns.Add(metaObjectColumn);
            settings.StandardColumns.Add(objectColumn);
            settings.StandardColumns.Add(rowNumberColumn);

            return settings;
        }

        private static MetaObjectKindSettings BuildDefaultOperationKindSettings()
        {
            var settings = new MetaObjectKindSettings()
            {
                Uid = Guid.Parse("{14A60875-E241-4E99-B32D-D45B2726D18B}"),
                Name = "operation",
                Prefix = "opr",
                Title = DictMain.Operaion,
                IconClass = "pi pi-file",
                StoreData = true,
                IsReference = true,
                AllowAttacheFiles = true,
                UseDetailsTables = true,
                CanCreateRecords = true,
                OrderByExpression = "number desc, period desc",
                DisplayExpression = $"{DictMain.Operaion} " + "#{{number}} " + DictMain.From.ToLower() + " {{date}}"
            };

            var numberColumn = new MetaObjectKindStandardColumn(Guid.Parse("{A39E7B83-E94D-4646-B620-04434FEF2F4C}"),
                                                                "number",
                                                                DictMain.Number,
                                                                DataSettingsBuilder.Make().DataType(DataTypeDefaults.Int).PrimaryKey(true).Build());

            var dateColumn = new MetaObjectKindStandardColumn(Guid.Parse("{F244DE36-53C9-4E93-A5A2-94ABBED91E02}"),
                                                              "date",
                                                              DictMain.Date,
                                                              DataSettingsBuilder.Make().DataType(DataTypeDefaults.DateTime).Required(true).Build());

            var createRecordsColumn = new MetaObjectKindStandardColumn(Guid.Parse("{FEE422DB-18DC-442A-A8DD-01DF76C20A98}"),
                                                                      "create_records",
                                                                      DictMain.CreateRecords,
                                                                      DataSettingsBuilder.Make().DataType(DataTypeDefaults.Bool).Build());


            settings.StandardColumns.Add(numberColumn);
            settings.StandardColumns.Add(dateColumn);
            settings.StandardColumns.Add(createRecordsColumn);

            settings.RecordsSettings.SourceCreateRecordsColumnUid = createRecordsColumn.Uid;
            settings.RecordsSettings.StorageMetaObjectKindUid = Guid.Parse(RecordsRegisterUid);

            settings.RecordsSettings.StoragePeriodColumnUid = Guid.Parse(RecordsColumnsIdentifiers.PeriodColumnUid);
            settings.RecordsSettings.StorageKindColumnUid = Guid.Parse(RecordsColumnsIdentifiers.ObjectKindColumnUid);
            settings.RecordsSettings.StorageMetaObjectColumnUid = Guid.Parse(RecordsColumnsIdentifiers.MetaObjectColumnUid);
            settings.RecordsSettings.StorageObjectColumnUid = Guid.Parse(RecordsColumnsIdentifiers.ObjectColumnUid);
            settings.RecordsSettings.StorageRowColumnUid = Guid.Parse(RecordsColumnsIdentifiers.RowColumnUid);

            return settings;
        }
      
    }
    
}
