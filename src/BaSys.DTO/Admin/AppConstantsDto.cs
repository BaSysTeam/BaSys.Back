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
    }
}
