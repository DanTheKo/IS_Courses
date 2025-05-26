namespace CourseService.Models.NotUsed
{
    public enum ContentType
    {
        Text,       // Простой текст (HTML/Markdown)
        Video,      // Ссылка на видео (YouTube/Vimeo/S3)
        Image,      // URL изображения или base64
        Test,       // JSON с вопросами/ответами
        File,       // Ссылка на скачивание файла
        Embed       // iframe или embed-код (например, Codepen)
    }
}
