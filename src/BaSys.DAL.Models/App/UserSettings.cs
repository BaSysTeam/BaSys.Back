using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class UserSettings
    {
        public Guid Uid { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Languages Language { get; set; }
    }
}
