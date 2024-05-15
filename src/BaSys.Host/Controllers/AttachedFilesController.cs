using BaSys.Common.Infrastructure;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Mvc;
using FileInfo = BaSys.Metadata.Models.FileInfo;

namespace BaSys.Host.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class AttachedFilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public AttachedFilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("GetAttachedFilesList")]
    public async Task<IActionResult> GetAttachedFilesList([FromQuery] string metaObjectKindUid,
        [FromQuery] string metaObjectUid,
        [FromQuery] string objectUid)
    {
        if (!Guid.TryParse(metaObjectUid, out var metaObjectGuid) ||
            !Guid.TryParse(metaObjectKindUid, out var metaObjectKindGuid) ||
            string.IsNullOrEmpty(objectUid))
            return BadRequest();
        
        var result = new ResultWrapper<List<FileInfo>>();

        var filesList = await _fileService.GetAttachedFilesList(metaObjectKindGuid, metaObjectGuid, objectUid);
        if (filesList != null)
            result.Success(filesList);
        
        return Ok(result);
    }
    
    [HttpPost("Upload")]
    public async Task<IActionResult> Upload(IFormCollection files,
        [FromQuery]string metaObjectKindUid,
        [FromQuery]string metaObjectUid,
        [FromQuery]string objectUid)
    {
        if (!Guid.TryParse(metaObjectUid, out var metaObjectGuid) ||
            !Guid.TryParse(metaObjectKindUid, out var metaObjectKindGuid) ||
            string.IsNullOrEmpty(objectUid))
            return BadRequest();
        
        foreach (var file in files.Files)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var uploadedFile = new FileUploadDto
            {
                ObjectUid = objectUid,
                MetaObjectUid = metaObjectGuid,
                MetaObjectKindUid = metaObjectKindGuid,
                FileName = file.FileName,
                MimeType = file.ContentType,
                Data = ms.ToArray()
            };
            
            await _fileService.UploadFileAsync(uploadedFile);
        }
        
        return Ok();
    }
}