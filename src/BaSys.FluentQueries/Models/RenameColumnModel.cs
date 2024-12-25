using System;
using System.Collections.Generic;
using System.Text;

namespace BaSys.FluentQueries.Models
{
    public class RenameColumnModel
    {
        public string OldName { get; set; } = string.Empty;
        public string NewName { get; set; } = string.Empty;
    }
}
