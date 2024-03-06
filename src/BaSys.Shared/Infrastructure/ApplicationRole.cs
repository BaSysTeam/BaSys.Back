using System.Collections.Generic;
using System.Linq;

namespace BaSys.Common.Infrastructure
{
    /// <summary>
    /// Stores names of asp.net roles, used in the application.
    /// </summary>
    public sealed class ApplicationRole
    {
        public const string SuperAdministrator = "sa";
        public const string Administrator = "administrator";
        public const string Designer = "designer";
        public const string User = "user";
        public const string Readonly = "readonly";

        public string Name { get; set; }
        public string Title { get; set; }

        public ApplicationRole()
        {

        }

        public ApplicationRole(string name, string title)
        {
            Name = name;
            Title = title;
        }

        /// <summary>
        /// Return all roles except SuperAdministrator.
        /// </summary>
        /// <returns></returns>
        public static IList<ApplicationRole> AllApplicationRoles()
        {
            var roles = new List<ApplicationRole>();

            roles.Add(new ApplicationRole(Administrator, "Administrator"));
            roles.Add(new ApplicationRole(Designer, "Designer"));
            roles.Add(new ApplicationRole(User, "User"));
            roles.Add(new ApplicationRole(Readonly, "Readonly"));

            return roles;
        }

        /// <summary>
        /// Return all role names except SuperAdministrator.
        /// </summary>
        /// <returns></returns>
        public static IList<string> AllApplicationRolesNames()
        {
            return AllApplicationRoles().Select(x => x.Name).ToList();
        }

        public override string ToString()
        {
            return $"{Name}";
        }

    }
}
