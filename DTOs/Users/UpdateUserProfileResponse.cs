namespace TutoringAcademy.DTOs.Users
{
    // This class represents the response data returned after updating a user's profile. It includes properties such as user ID, name, username, avatar URL, email, and an optional contact field.
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