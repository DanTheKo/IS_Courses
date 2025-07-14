namespace GatewayAPI.Models.DTO
{
    public class ProfileDto
    {
        public string id { get; set; }
        public string identityId { get; set; }
        public string firstname { get; set; } = string.Empty;
        public string lastname { get; set; } = string.Empty;
        public string middlename { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string profileImageUrl { get; set; } = string.Empty;
    }
}
