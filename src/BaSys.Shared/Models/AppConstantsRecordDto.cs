using BaSys.Common.Models;

namespace BaSys.Common.Models
{
    public class AppConstantsRecordDto
    {
        public string Uid { get; set; } = string.Empty;
        public string DataBaseUid { get; set; } = string.Empty;
        public string ApplicationTitle { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;

        public AppConstantsRecordDto()
        {
            
        }

        public AppConstantsRecordDto(AppConstantsRecord source)
        {
            if (source == null)
                return;

            Uid = source.Uid.ToString();
            DataBaseUid = source.DataBaseUid.ToString();
            ApplicationTitle = source.ApplicationTitle;
        }
    }
}
