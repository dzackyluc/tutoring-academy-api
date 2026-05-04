namespace TutoringAcademy.DTOs.Users
{
    // This class represents the response data returned after a successful user login. It includes properties such as token, user ID, name, avatar URL, username, and email.
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? AvatarUrl { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}