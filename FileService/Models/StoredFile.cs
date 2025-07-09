namespace FileService.Models
{
    public class StoredFile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string FileName { get; set; }
        public string StoredFileName { get; set; }  // Имя файла на диске (Guid + расширение)
        public string ContentType { get; set; }
        public long SizeInBytes { get; set; }
        public DateTime UploadTime { get; set; } = DateTime.UtcNow;
    }
}
