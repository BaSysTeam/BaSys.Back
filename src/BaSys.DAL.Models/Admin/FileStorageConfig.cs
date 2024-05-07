using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.DAL.Models.Admin
{
    public sealed class FileStorageConfig
    {
        public Guid Uid { get; set; } = Guid.NewGuid();
        public string S3ConnectionString { get; set; } = string.Empty;
        public int MaxFileSizeMb { get; set; } = 50;
        public bool IsEnabled { get; set; }
    }
}
