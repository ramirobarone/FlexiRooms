namespace Infrastructure.Models;

public class UserProfileImage
{
    public string UserId { get; set; } = string.Empty;
    public byte[] Content { get; set; } = [];
    public string ContentType { get; set; } = "application/octet-stream";
    public string FileName { get; set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
