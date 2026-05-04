namespace TutoringAcademy.DTOs.Users
{
    // This class represents the input data required for user registration. It includes properties such as name, username, email, password, and an optional contact field.
    public class RegisterInput
    {
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Contact { get; set; }
    }
}