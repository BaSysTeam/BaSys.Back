using BaSys.Admin.Abstractions;
using BaSys.Admin.DTO;
using BaSys.Common.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.Admin.Controllers;

[Route("api/admin/v1/[controller]")]
[ApiController]
// [Authorize(Roles = ApplicationRole.Administrator)]
public class UserGroupController : ControllerBase
{
    private readonly IUserGroupService _userGroupService;
    
    public UserGroupController(IUserGroupService userGroupService)
    {
        _userGroupService = userGroupService;
    }

    /// <summary>
    /// Get user group list
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetUserGroupList()
    {
        var result = await _userGroupService.GetUserGroupList();
        return Ok(result);
    }
    
    [HttpGet("GetUserGroup")]
    public async Task<IActionResult> GetUserGroup([FromQuery] Guid userGroupUid)
    {
        var result = await _userGroupService.GetUserGroup(userGroupUid);
        return Ok(result);
    }
    
    /// <summary>
    /// Delete user group
    /// </summary>
    /// <param name="userGroupUid"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteUserGroup([FromQuery] Guid userGroupUid)
    {
        var result = await _userGroupService.DeleteUserGroup(userGroupUid);
        return Ok(result);
    }
    
    /// <summary>
    /// Create empty user group
    /// </summary>
    /// <param name="userGroup"></param>
    /// <returns></returns>
    [HttpPost("Create")]
    public async Task<IActionResult> CreateUserGroups([FromBody] UserGroupDto userGroup)
    {
        var result = await _userGroupService.CreateUserGroup(userGroup);
        return Ok(result);
    }
    
    /// <summary>
    /// Save user group
    /// </summary>
    /// <param name="userGroup"></param>
    /// <returns></returns>
    [HttpPost("Save")]
    public async Task<IActionResult> Save([FromBody] UserGroupDetailDto userGroup)
    {
        var result = await _userGroupService.SaveUserGroup(userGroup);
        return Ok(result);
    }

    [HttpPost("Test")]
    public async Task<IActionResult> Test()
    {
        var userGroup = new UserGroupDetailDto
        {
            Uid = new Guid("2C879D09-5D7E-4A20-995A-946EEE98E632"),
            Name = "Test user group 2",
            Memo = "Test user group 2 memo",
            CreateDate = DateTime.Now,
            Users = new List<UserGroupUserDto>
            {
                new()
                {
                    UserUid = new Guid("daee587f-fbbd-4653-8e5a-4f3402be7b45")
                }
            },
            Roles = new List<UserGroupRoleDto>
            {
                // new()
                // {
                //     RoleUid = new Guid("5483f8ff-ebf8-4cc8-8b4b-4b989886f452")
                // },
                new()
                {
                    RoleUid = new Guid("5a885da4-6d95-43b1-91ec-a23e498f6dd5")
                }
            }
        };
        
        await _userGroupService.SaveUserGroup(userGroup);
        return Ok();
    }
}