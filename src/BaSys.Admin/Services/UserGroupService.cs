using System.Collections;
using System.Data;
using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using BaSys.Constructor.Abstractions;
using BaSys.DAL.Models.App;
using BaSys.Host.DAL.Abstractions;
using BaSys.Host.DAL.DataProviders;
using BaSys.Host.Identity.Models;
using BaSys.Metadata.DTOs;
using BaSys.Metadata.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaSys.Admin.Services;

internal class ChangesModel
{
    public IEnumerable<Guid>? ToInsert { get; set; }
    public IEnumerable<Guid>? ToDelete { get; set; }
}

public class UserGroupService : IUserGroupService, IDisposable
{
    private readonly IDbConnection _connection;
    private readonly UserGroupProvider _userGroupProvider;
    private readonly UserGroupUserProvider _userGroupUserProvider;
    private readonly UserGroupRoleProvider _userGroupRoleProvider;
    private readonly UserGroupRightProvider _userGroupRightProvider;
    private readonly RoleManager<WorkDbRole> _roleManager;
    private readonly UserManager<WorkDbUser> _userManager;
    private readonly IMetaObjectKindsService _metaObjectKindsService;
    private readonly IMetaObjectsService _metaObjectsService;

    public UserGroupService(IMainConnectionFactory mainConnectionFactory,
        ISystemObjectProviderFactory providerFactory,
        RoleManager<WorkDbRole> roleManager,
        UserManager<WorkDbUser> userManager,
        IMetaObjectKindsService metaObjectKindsService,
        IMetaObjectsService metaObjectsService)
    {
        _connection = mainConnectionFactory.CreateConnection();
        _roleManager = roleManager;
        _userManager = userManager;
        _metaObjectKindsService = metaObjectKindsService;
        _metaObjectsService = metaObjectsService;

        providerFactory.SetUp(_connection);
        _userGroupProvider = providerFactory.Create<UserGroupProvider>();
        _userGroupUserProvider = providerFactory.Create<UserGroupUserProvider>();
        _userGroupRoleProvider = providerFactory.Create<UserGroupRoleProvider>();
        _userGroupRightProvider = providerFactory.Create<UserGroupRightProvider>();
    }

    public async Task<ResultWrapper<IEnumerable<UserGroupDto>?>> GetUserGroupList()
    {
        var result = new ResultWrapper<IEnumerable<UserGroupDto>?>();
        var userGroups = (await _userGroupProvider.GetCollectionAsync(null))
            .OrderBy(x => x.CreateDate)
            .Select(x => new UserGroupDto(x));

        result.Success(userGroups);
        return result;
    }

    public async Task<ResultWrapper<bool>> CreateUserGroup(UserGroupDto userGroup)
    {
        var result = new ResultWrapper<bool>();
        var model = userGroup.ToModel();
        model.CreateDate = DateTime.UtcNow;
        var r = await _userGroupProvider.InsertAsync(model, null);

        if (r == 1)
            result.Success(true);
        else
            result.Error(-1, "Error create UserGroup");

        return result;
    }

    public async Task<ResultWrapper<UserGroupDetailDto>> SaveUserGroup(UserGroupDetailDto userGroup)
    {
        var result = new ResultWrapper<UserGroupDetailDto>();

        _connection.Open();
        var transaction = _connection.BeginTransaction();

        // Main
        if (userGroup.Uid == Guid.Empty)
        {
            await _userGroupProvider.InsertAsync(new UserGroup
            {
                Name = userGroup.Name ?? string.Empty,
                Memo = userGroup.Memo,
                CreateDate = DateTime.UtcNow
            }, transaction);

            // ToDo: get inserted guid!!
            userGroup.Uid = Guid.NewGuid();
        }
        else
        {
            await _userGroupProvider.UpdateAsync(new UserGroup
            {
                Uid = userGroup.Uid,
                Name = userGroup.Name ?? string.Empty,
                Memo = userGroup.Memo,
                CreateDate = userGroup.CreateDate
            }, transaction);
        }

        // Users
        if (userGroup.Users?.Any() == true)
            await SaveUsers(userGroup.Uid, userGroup.Users, transaction);

        // Roles
        if (userGroup.Roles?.Any() == true)
            await SaveRoles(userGroup.Uid, userGroup.Roles, transaction);
        
        // Global rights
        if (userGroup.GlobalRights?.Any() == true)
            await SaveRights(userGroup.Uid, userGroup.GlobalRights, transaction);
        
        // Rights
        if (userGroup.Rights?.Any() == true)
            await SaveRights(userGroup.Uid, userGroup.Rights, transaction);

        transaction.Commit();
        return result;
    }

    public async Task<ResultWrapper<bool>> DeleteUserGroup(Guid userGroupUid)
    {
        var result = new ResultWrapper<bool>();
        var r = await _userGroupProvider.DeleteAsync(userGroupUid, null);
        if (r == 1)
            result.Success(true);
        else
            result.Error(-1, "Error delete UserGroup");

        return result;
    }

    public async Task<ResultWrapper<UserGroupDetailDto>> GetUserGroup(Guid userGroupUid)
    {
        var result = new ResultWrapper<UserGroupDetailDto>();

        var userGroupDetail = new UserGroupDetailDto();

        var metaObjectKinds = (await _metaObjectKindsService.GetCollectionAsync())?.Data.ToList();

        var userGroup = await _userGroupProvider.GetItemAsync(userGroupUid, null);
        if (userGroup == null)
        {
            result.Error(-1, "user group not found!");
            return result;
        }

        // Main
        userGroupDetail.Uid = userGroup.Uid;
        userGroupDetail.Name = userGroup.Name;
        userGroupDetail.Memo = userGroup.Memo;
        userGroupDetail.CreateDate = userGroup.CreateDate;
        // Users
        userGroupDetail.Users = await GetUsers(userGroupUid);
        // Roles
        userGroupDetail.Roles = await GetRoles(userGroupUid);

        if (metaObjectKinds?.Any() == true)
        {
            var rights = (await _userGroupRightProvider.GetCollectionByUserGroupUidAsync(userGroupUid)).ToList();
            // Global rights
            userGroupDetail.GlobalRights = GetGlobalRights(metaObjectKinds, rights);
            // Rights by MetaObjectKinds
            userGroupDetail.Rights = await GetRights(metaObjectKinds, rights);
        }

        result.Success(userGroupDetail);
        return result;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    #region private methods

    private async Task<List<UserGroupUserDto>?> GetUsers(Guid userGroupUid)
    {
        var users = new List<UserGroupUserDto>();

        var dbUsers = await _userManager.Users.ToListAsync();
        foreach (var dbUser in dbUsers)
        {
            users.Add(new UserGroupUserDto
            {
                UserUid = Guid.Parse(dbUser.Id),
                UserName = dbUser.UserName
            });
        }

        var items = (await _userGroupUserProvider.GetCollectionByUserGroupUidAsync(userGroupUid)).ToList();
        foreach (var user in users.Where(u => items.Select(x => x.UserUid).Contains(u.UserUid)))
        {
            user.Uid = items.First(x => x.UserUid == user.UserUid).Uid;
            user.IsChecked = true;
        }

        return users;
    }

    private async Task<List<UserGroupRoleDto>> GetRoles(Guid userGroupUid)
    {
        var roles = new List<UserGroupRoleDto>();

        var dbRoles = await _roleManager.Roles.ToListAsync();
        foreach (var dbRole in dbRoles)
        {
            roles.Add(new UserGroupRoleDto
            {
                RoleUid = Guid.Parse(dbRole.Id),
                Name = dbRole.Name
            });
        }

        var items = (await _userGroupRoleProvider.GetCollectionByUserGroupUidAsync(userGroupUid)).ToList();
        foreach (var role in roles.Where(u => items.Select(x => x.RoleUid).Contains(u.RoleUid)))
        {
            role.Uid = items.First(x => x.RoleUid == role.RoleUid).Uid;
            role.IsChecked = true;
        }

        return roles;
    }

    private List<UserGroupRightDto> GetGlobalRights(List<MetaObjectKind> metaObjectKinds, List<UserGroupRight> rights)
    {
        var globalRights = new List<UserGroupRightDto>();
        foreach (var globalRight in ApplicationRightDefaults.AllRights().Where(x => x.IsGlobal))
        {
            foreach (var metaObjectKind in metaObjectKinds)
            {
                globalRights.Add(new UserGroupRightDto
                {
                    RightUid = globalRight.Uid,
                    RightName = $"{globalRight.Title}: {metaObjectKind.Title}",
                    MetaObjectKindUid = metaObjectKind.Uid
                });
            }
        }

        foreach (var right in globalRights.Where(u => rights.Select(x => x.RightUid).Contains(u.RightUid)))
        {
            right.Uid = rights.First(x => x.RightUid == right.RightUid).Uid;
            right.IsChecked = true;
        }

        return globalRights;
    }

    private async Task<List<UserGroupRightDto>> GetRights(List<MetaObjectKind> metaObjectKinds,
        List<UserGroupRight> dbRights)
    {
        var rights = new List<UserGroupRightDto>();
        foreach (var metaObjectKind in metaObjectKinds)
        {
            var metaObjects = (await _metaObjectsService.GetMetaObjectsAsync(metaObjectKind.Name))?.Data;
            foreach (var metaObject in metaObjects ?? Enumerable.Empty<MetaObjectStorableSettingsDto>())
            {
                foreach (var right in ApplicationRightDefaults.AllRights().Where(x => !x.IsGlobal))
                {
                    rights.Add(new UserGroupRightDto
                    {
                        RightUid = right.Uid,
                        RightName = right.Title,
                        MetaObjectUid = Guid.Parse(metaObject.Uid),
                        MetaObjectTitle = metaObject.Title,
                        MetaObjectKindUid = metaObjectKind.Uid
                    });
                }
            }
        }

        foreach (var right in rights.Where(u => dbRights.Select(x => x.RightUid).Contains(u.RightUid)))
        {
            right.Uid = dbRights.First(x => x.RightUid == right.RightUid).Uid;
            right.IsChecked = true;
        }

        return rights;
    }

    private async Task SaveUsers(Guid userGroupUid, List<UserGroupUserDto> userGroupUsers, IDbTransaction transaction)
    {
        var items = (await _userGroupUserProvider.GetCollectionByUserGroupUidAsync(userGroupUid, transaction)).ToList();
        var changes = GetChanges(items.Select(x => x.UserUid), userGroupUsers.Select(x => x.UserUid));

        // Delete
        if (changes.ToDelete?.Any() == true)
        {
            foreach (var uid in changes.ToDelete)
            {
                var item = items.First(x => x.UserUid == uid);
                await _userGroupUserProvider.DeleteAsync(item.Uid, transaction);
            }
        }

        // Insert
        if (changes.ToInsert?.Any() == true)
        {
            foreach (var uid in changes.ToInsert)
            {
                await _userGroupUserProvider.InsertAsync(new UserGroupUser
                {
                    UserUid = uid,
                    UserGroupUid = userGroupUid
                }, transaction);
            }
        }
    }

    private async Task SaveRoles(Guid userGroupUid, List<UserGroupRoleDto> userGroupRoles, IDbTransaction transaction)
    {
        var items = (await _userGroupRoleProvider.GetCollectionByUserGroupUidAsync(userGroupUid, transaction)).ToList();
        var changes = GetChanges(items.Select(x => x.RoleUid), userGroupRoles.Select(x => x.RoleUid));

        // Delete
        if (changes.ToDelete?.Any() == true)
        {
            foreach (var uid in changes.ToDelete)
            {
                var item = items.First(x => x.RoleUid == uid);
                await _userGroupRoleProvider.DeleteAsync(item.Uid, transaction);
            }
        }

        // Insert
        if (changes.ToInsert?.Any() == true)
        {
            foreach (var uid in changes.ToInsert)
            {
                await _userGroupRoleProvider.InsertAsync(new UserGroupRole
                {
                    RoleUid = uid,
                    UserGroupUid = userGroupUid
                }, transaction);
            }
        }
    }

    private async Task SaveRights(Guid userGroupUid, List<UserGroupRightDto> rights, IDbTransaction transaction)
    {
        var items = (await _userGroupRightProvider.GetCollectionByUserGroupUidAsync(userGroupUid, transaction)).ToList();
        var changes = GetChanges(items.Select(x => x.RightUid), rights.Select(x => x.RightUid));

        // Delete
        if (changes.ToDelete?.Any() == true)
        {
            foreach (var uid in changes.ToDelete)
            {
                var item = items.First(x => x.RightUid == uid);
                await _userGroupRightProvider.DeleteAsync(item.Uid, transaction);
            }
        }

        // Insert
        if (changes.ToInsert?.Any() == true)
        {
            foreach (var uid in changes.ToInsert)
            {
                await _userGroupRightProvider.InsertAsync(new UserGroupRight
                {
                    RightUid = uid,
                    UserGroupUid = userGroupUid
                }, transaction);
            }
        }
    }

    private ChangesModel GetChanges(IEnumerable<Guid> dbUids, IEnumerable<Guid> modelUids)
    {
        var toDelete = new List<Guid>();
        var toInsert = new List<Guid>();

        var modelUidsList = modelUids.ToList();
        var dbUidsList = dbUids.ToList();

        foreach (var dbUid in dbUidsList)
        {
            if (!modelUidsList.Any(x => x == dbUid))
                toDelete.Add(dbUid);
        }

        foreach (var modelUid in modelUidsList)
        {
            if (!dbUidsList.Any(x => x == modelUid))
                toInsert.Add(modelUid);
        }

        return new ChangesModel
        {
            ToInsert = toInsert,
            ToDelete = toDelete
        };
    }

    #endregion
}