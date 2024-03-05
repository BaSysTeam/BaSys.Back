using BaSys.Admin.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaSys.Admin.UnitTests
{
    [TestFixture]
    internal class UserDtoTests
    {
        [Test]
        public void AddRoles_OneRole_RolesList()
        {
            var roles = new List<string>
            {
                "user"
            };

            var userDto = new UserDto();
            userDto.AddRoles(roles);

            var checkedRoles = userDto.Roles.Where(x=>x.IsChecked).ToList();    

            Assert.That(checkedRoles.Count, Is.EqualTo(1));
            Assert.That(roles[0], Is.EqualTo(checkedRoles[0].Name));
        }

        [Test]
        public void AddRoles_TwoRole_RolesList()
        {
            var roles = new List<string>
            {
                "user",
                "administrator"
            };

            var userDto = new UserDto();
            userDto.AddRoles(roles);

            var checkedRoles = userDto.Roles.Where(x => x.IsChecked).ToList();

            Assert.That(checkedRoles.Count, Is.EqualTo(2));
          
        }

        [Test]
        public void AddRoles_NoRole_EmptyRolesList()
        {
            var roles = new List<string>();

            var userDto = new UserDto();
            userDto.AddRoles(roles);

            var checkedRoles = userDto.Roles.Where(x => x.IsChecked).ToList();

            Assert.That(checkedRoles.Count, Is.EqualTo(0));
           
        }

        [Test]
        public void AddRoles_WrongRole_EmptyRolesList()
        {
            var roles = new List<string>()
            {
                "wrong"
            };

            var userDto = new UserDto();
            userDto.AddRoles(roles);

            var checkedRoles = userDto.Roles.Where(x => x.IsChecked).ToList();

            Assert.That(checkedRoles.Count, Is.EqualTo(0));

        }
    }
}
