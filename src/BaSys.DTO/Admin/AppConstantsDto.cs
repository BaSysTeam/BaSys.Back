using BaSys.DAL.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DTO.Admin
{
    public sealed class AppConstantsDto
    {
        public Guid Uid { get; set; }
        public Guid DataBaseUid { get; set; }
        public string ApplicationTitle { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;

        public AppConstantsDto()
        {
        }

        public AppConstantsDto(AppConstants model)
        {
            Uid = model.Uid;
            DataBaseUid = model.DataBaseUid;
            ApplicationTitle = model.ApplicationTitle;
        }

        public AppConstants ToModel()
        {
            return new AppConstants
            {
                Uid = Uid,
                DataBaseUid = DataBaseUid,
                ApplicationTitle = ApplicationTitle
            };
        }
    }
}
