using BaSys.FileStorage.Abstractions;
using BaSys.Host.Abstractions;
using BaSys.Host.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using FileInfo = BaSys.Metadata.Models.FileInfo;

namespace BaSys.Host.Controllers;
[Route("api/v1/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly IFileService _fileService;
    
    public TestController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        var byteArr = await System.IO.File.ReadAllBytesAsync(@"D:\Test\cat.jpg");
        await _fileService.UploadFileAsync(new FileUploadDto
        {
            Data = byteArr,
            FileName = "test image",
            MimeType = "image/jpeg",
            IsImage = true,
            IsMainImage = true,
            MetaObjectKindUid = new Guid("e601e899-5aaa-4699-a6bd-7f3d684e29a3"),
            MetaObjectUid = new Guid("4730C14E-12C3-4512-90E1-68D325859172"),
            ObjectUid = "1"
        });
        
        return Ok();
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        await _fileService.RemoveFileAsync(new FileInfo
        {
            Uid = new Guid("594E570B-689C-4F7F-96EC-B8C882A9B99B"),
            FileName = "test image",
            MimeType = "image",
            IsImage = true,
            IsMainImage = true,
            MetaObjectKindUid = new Guid("e601e899-5aaa-4699-a6bd-7f3d684e29a3"),
            MetaObjectUid = new Guid("4730C14E-12C3-4512-90E1-68D325859172")
        });
        
        return Ok();
    }

    [HttpGet("GetAttachedFilesList")]
    public async Task<IActionResult> GetAttachedFilesList()
    {
        var list = await _fileService.GetAttachedFilesList(new Guid("e601e899-5aaa-4699-a6bd-7f3d684e29a3"), new Guid("4730C14E-12C3-4512-90E1-68D325859172"), "1");
        return Ok(list);
    }
}