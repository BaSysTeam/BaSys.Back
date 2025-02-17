using BaSys.Common.Abstractions;
using BaSys.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.App
{
    public sealed class UserSettings: SystemObjectBase
    {
        public string UserId { get; set; } = string.Empty;
        public Languages Language { get; set; }
    }
}
