namespace TutoringAcademy.DTOs.Users
{
    // This class represents the response data returned after a successful user registration. It includes properties such as token, user ID, name, avatar URL, username, email, and an optional contact field.
    public class RegisterResponse
    {
        public string Token { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Contact { get; set; }
    }
}