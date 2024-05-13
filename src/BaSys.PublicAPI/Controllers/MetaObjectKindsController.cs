using BaSys.Constructor.Abstractions;
using BaSys.Logging.Abstractions.Abstractions;
using BaSys.Logging.EventTypes;
using BaSys.Logging.Infrastructure;
using BaSys.PublicAPI.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaSys.PublicAPI.Controllers;

[Route("api/public/v1/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MetaObjectKindsController : ControllerBase
{
    private readonly IMetaObjectKindsService _metaObjectKindsService;
    private readonly ILoggerService _basysLogger;

    public MetaObjectKindsController(IMetaObjectKindsService metaObjectKindsService,
        ILoggerService basysLogger)
    {
        _metaObjectKindsService = metaObjectKindsService;
        _basysLogger = basysLogger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMetaObjectKinds()
    {
        _basysLogger.Info(PublicApiLogHelper.GetMessage(HttpContext), EventTypeFactory.PublicApi);
        var result = await _metaObjectKindsService.GetCollectionAsync();
        return Ok(result);
    }
    
    [HttpGet("{uid:guid}")]
    public async Task<IActionResult> GetMetaObjectKinds(Guid uid)
    {
        _basysLogger.Info(PublicApiLogHelper.GetMessage(HttpContext), EventTypeFactory.PublicApi);
        var result = await _metaObjectKindsService.GetSettingsItemAsync(uid);
        return Ok(result);
    }
    
    [HttpGet("{name}")]
    public async Task<IActionResult> GetMetaObjectKinds(string name)
    {
        var result = await _metaObjectKindsService.GetSettingsItemByNameAsync(name);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> PostTest([FromBody]string name)
    {
        var logMessage = PublicApiLogHelper.GetMessage(HttpContext, new()
        {
            {"name", name}
        });
        _basysLogger.Info(logMessage, EventTypeFactory.PublicApi);
        return Ok();
    }
}