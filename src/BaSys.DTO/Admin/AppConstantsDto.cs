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
        public string Uid { get; set; } = string.Empty;
        public string DataBaseUid { get; set; } = string.Empty;
        public string ApplicationTitle { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;

        public AppConstantsDto()
        {
            
        }

        public AppConstantsDto(AppConstants model)
        {
            Uid = model.Uid.ToString();
            DataBaseUid = model.DataBaseUid.ToString();
            ApplicationTitle = model.ApplicationTitle;
        }

        public AppConstants ToModel()
        {
            var model = new AppConstants
            {
                ApplicationTitle = ApplicationTitle
            };

            if (Guid.TryParse(Uid, out var uid))
                model.Uid = uid;
            if (Guid.TryParse(DataBaseUid, out var dataBaseUid))
                model.DataBaseUid = dataBaseUid;

            return model;
        }
    }
}
