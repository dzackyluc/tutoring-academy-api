namespace TutoringAcademy.DTOs.Users
{
    public class UpdateUserProfileResponse
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Contact { get; set; }
    }
}