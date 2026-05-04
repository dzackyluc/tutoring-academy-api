namespace TutoringAcademy.DTOs.Users
{
    public class UpdateUserProfileInput
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Contact { get; set; }
    }
}