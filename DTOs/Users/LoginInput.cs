namespace TutoringAcademy.DTOs.Users
{
    // This class represents the input data required for user login. It includes properties for the username and password.
    public class LoginInput
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}