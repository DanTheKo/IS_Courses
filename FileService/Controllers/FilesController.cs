using FileService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileStorageService _fileStorage;

        public FilesController(FileStorageService fileStorage)
        {
            _fileStorage = fileStorage;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                var storedFile = await _fileStorage.SaveFileAsync(file);
                return Ok(storedFile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
        {
            try
            {
                var fileBytes = await _fileStorage.GetFileAsync(fileName);
                return File(fileBytes, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("list")]
        public IActionResult ListFiles()
        {
            var files = _fileStorage.GetAllFiles();
            return Ok(files);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
            try
            {
                await _fileStorage.DeleteFileAsync(fileName);
                return NoContent();
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
