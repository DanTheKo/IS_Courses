using System.IO;
using FileService.Models;
using Microsoft.Extensions.Configuration;


namespace FileService.Services
{

    public class FileStorageService
    {
        private readonly string _storagePath;
        private readonly long _maxFileSizeBytes;

        public FileStorageService()
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(),
                                      "UploadedFiles");
            Console.WriteLine(_storagePath);
            _maxFileSizeBytes = 10 * 1024 * 1024;

            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }

        }

        public async Task<StoredFile> SaveFileAsync(IFormFile file)
        {
            if (file.Length > _maxFileSizeBytes)
                throw new Exception("File is too large");

            var fileExtension = Path.GetExtension(file.FileName);
            var storedFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_storagePath, storedFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new StoredFile
            {
                FileName = file.FileName,
                StoredFileName = storedFileName,
                ContentType = file.ContentType,
                SizeInBytes = file.Length
            };
        }

        public async Task<byte[]> GetFileAsync(string storedFileName)
        {
            var filePath = Path.Combine(_storagePath, storedFileName);
            return await File.ReadAllBytesAsync(filePath);
        }

        public async Task DeleteFileAsync(string storedFileName)
        {
            var filePath = Path.Combine(_storagePath, storedFileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public List<StoredFile> GetAllFiles()
        {
            return Directory.GetFiles(_storagePath)
                .Select(filePath => new FileInfo(filePath))
                .Select(fileInfo => new StoredFile
                {
                    StoredFileName = fileInfo.Name,
                    FileName = fileInfo.Name,  // Можно хранить оригинальное имя в БД
                    SizeInBytes = fileInfo.Length,
                    UploadTime = fileInfo.CreationTimeUtc
                })
                .ToList();
        }
    }
}
