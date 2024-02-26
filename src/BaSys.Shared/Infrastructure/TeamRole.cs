using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace BaSys.Common.Infrastructure
{
    /// <summary>
    /// Stores names of asp.net roles, used in the application.
    /// </summary>
    public sealed class TeamRole
    {
        public const string SuperAdministrator = "sa";
        public const string Administrator = "administrator";
        public const string Designer = "designer";
        public const string User = "user";
        public const string Readonly = "readonly";

        public string Name { get; set; }
        public string Title { get; set; }

        public TeamRole()
        {

        }

        public TeamRole(string name, string title)
        {
            Name = name;
            Title = title;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
