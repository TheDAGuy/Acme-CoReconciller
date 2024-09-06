using Microsoft.AspNetCore.Mvc;
using Acme_CoReconciller.Google_Drive_Files;

[ApiController]
[Route("api/[controller]")]
public class GoogleDriveController : ControllerBase
{
    private readonly Activity _driveservice;

    public GoogleDriveController(Activity driveservice)
    {
        _driveservice = driveservice;
    }

    [HttpGet("list-files")]
    public async Task<IActionResult> ListFiles()
    {
        var files = await _driveservice.ListFilesAsync();

        var fileList = files.Select(file => new
        {
            file.Id,
            file.Name
        }).ToList();

        return Ok(fileList);
    }
}

