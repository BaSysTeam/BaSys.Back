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
    public async Task<IActionResult> Upload(IFormCollection files)
    {
        if (!Guid.TryParse(files["metaObjectUid"], out var metaObjectUid) ||
            !Guid.TryParse(files["metaObjectKindUid"], out var metaObjectKindUid) ||
            string.IsNullOrEmpty(files["objectUid"]))
            return BadRequest();
        
        foreach (var file in files.Files)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var uploadedFile = new FileUploadDto
            {
                ObjectUid = files["objectUid"],
                MetaObjectUid = metaObjectUid,
                MetaObjectKindUid = metaObjectKindUid,
                FileName = file.FileName,
                MimeType = file.ContentType,
                Data = ms.ToArray()
            };
            
            await _fileService.UploadFileAsync(uploadedFile);
        }
        
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery]Guid metaObjectKindUid, [FromQuery]Guid fileUid)
    {
        var result = new ResultWrapper<bool>();
        if (await _fileService.RemoveFileAsync(metaObjectKindUid, fileUid))
            result.Success(true);
        else
            result.Error(-1, "Error delete file");
        
        return Ok(result);
    }
    
    [HttpGet("Download")]
    public async Task<IActionResult> Download([FromQuery] Guid metaObjectKindUid, [FromQuery] Guid fileUid)
    {
        var file = await _fileService.GetFileAsync(metaObjectKindUid, fileUid);
        if (file?.Data == null)
            return NotFound();
        
        var stream = new MemoryStream(file.Data);
        return File(stream, "application/octet-stream", file.FileName);
    }
    
    [HttpGet("GetImageBase64")]
    public async Task<IActionResult> GetImageBase64([FromQuery] Guid metaObjectKindUid, [FromQuery] Guid fileUid)
    {
        var result = new ResultWrapper<string>();
        var imageBase64 = await _fileService.GetImageBase64(fileUid);
        if (!string.IsNullOrEmpty(imageBase64))
            result.Success(imageBase64);
        else
            result.Error(-1, "Error load image");
        
        return Ok(result);
    }
}