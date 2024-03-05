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

        /// <summary>
        /// Return all roles except SuperAdministrator.
        /// </summary>
        /// <returns></returns>
        public static IList<TeamRole> AllApplicationRoles()
        {
            var roles = new List<TeamRole>();

            roles.Add(new TeamRole(Administrator, "Administrator"));
            roles.Add(new TeamRole(Designer, "Designer"));
            roles.Add(new TeamRole(User, "User"));
            roles.Add(new TeamRole(Readonly, "Readonly"));

            return roles;
        }

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
