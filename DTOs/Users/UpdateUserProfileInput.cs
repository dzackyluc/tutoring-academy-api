namespace TutoringAcademy.DTOs.Users
{
    // This class represents the input data required to update a user's profile. It includes properties such as name, username, email, and an optional contact field.
    public class UpdateUserProfileInput
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Contact { get; set; }
    }
}