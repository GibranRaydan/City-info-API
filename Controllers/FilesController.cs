using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
namespace CityInfo.API.Controllers;

[ApiController]
[Authorize]
[Route("api/files")]

public class FilesController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FilesController(
        FileExtensionContentTypeProvider fileExtensionContentTypeProvider
    ){
        _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
            ?? throw new System.ArgumentException(
                nameof(fileExtensionContentTypeProvider));
    }

    [HttpGet("{fileId}")]

    public ActionResult GetFile(string fileId)
    {
        var pathToFile = "triforce.png";

        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound("File not found");
        }

        if (!_fileExtensionContentTypeProvider.TryGetContentType(
            pathToFile, out var contentType
        ))
        {
            contentType = "application/ocnet-stream";
        }

        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, contentType, Path.GetFileName(pathToFile));
    }
}