using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaSys.Translation;

namespace BaSys.Metadata.Models
{
    public static class ApplicationRightDefaults
    {

        public static ApplicationRight FullAccessForMetaObjectKindRight = new()
        {
            Uid = new Guid("{216606B4-BCAC-4445-AE33-B0888A94E3E5}"),
            Title = DictMain.FullAccessForMetaObjectKindRight,
            Name = "full_access",
            IsGlobal = true,
        };

        public static ApplicationRight ReadRight = new()
        {
            Uid = new Guid("{216606B4-BCAC-4445-AE33-B0888A94E3E5}"),
            Title = DictMain.ReadRight,
            Name = "read"
        };

        public static ApplicationRight AddRight = new()
        {
            Uid = new Guid("{7A0224D9-EB5E-44D7-BC5E-1F20C6412844}"),
            Title = DictMain.AddRight,
            Name = "add"
        };

        public static ApplicationRight EditRight = new()
        {
            Uid = new Guid("{07C610BC-6F65-4905-BE5F-8389F5B6E1AE}"),
            Title = DictMain.EditRight,
            Name = "edit"
        };

        public static ApplicationRight DeleteRight = new()
        {
            Uid = new Guid("{15CBC076-119C-4A26-86E9-F79368C41222}"),
            Title = DictMain.DeleteRight,
            Name = "delete"
        };

        public static ApplicationRight AddFilesRight = new()
        {
            Uid = new Guid("{0E3AD6C4-BA26-485F-AEDE-6B8405A55B69}"),
            Title = DictMain.AddFilesRight,
            Name = "add_files"
        };

        public static ApplicationRight DeleteFilesRight = new()
        {
            Uid = new Guid("{3C002342-6C49-47C5-957E-802F71A66939}"),
            Title = DictMain.DeleteFilesRight,
            Name = "delete_files"
        };

        public static IList<ApplicationRight> AllRights()
        {
            var collection = new List<ApplicationRight>()
            {
                FullAccessForMetaObjectKindRight,
                ReadRight,
                AddRight,
                EditRight,
                DeleteRight,
                AddFilesRight,
                DeleteFilesRight,
            };

            return collection;
        }
    }
}
