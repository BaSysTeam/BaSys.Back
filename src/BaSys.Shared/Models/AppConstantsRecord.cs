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
    }
}
