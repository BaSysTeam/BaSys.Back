using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.Common.Models
{
    public sealed class AppConstantsRecord
    {
        public Guid Uid { get; set; }
        public Guid DataBaseUid { get; set; }
        public string ApplicationTitle { get; set; }

        public AppConstantsRecord()
        {
            
        }

        public AppConstantsRecord(AppConstantsRecordDto source)
        {
            if (source == null)
                return;

            if (Guid.TryParse(source.Uid, out var uid))
                Uid = uid;

            if (Guid.TryParse(source.DataBaseUid, out var dataBaseUid))
                DataBaseUid = dataBaseUid;

            ApplicationTitle = source.ApplicationTitle;
        }
    }
}
