using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TutoringAcademy.Models
{
    // This class represents a user in the tutoring academy.
    // It includes properties such as name, username, email, role, contact information, password hash, and lists of enrolled and teaching courses.
    // The UserRole enumeration defines the different roles a user can have in the system, such as Admin, User, and Tutor, which can be used for authorization and access control throughout the application.
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("username")]
        public string Username { get; set; } = null!;

        [BsonElement("avatarUrl")]
        public string AvatarUrl { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("role")]
        [BsonRepresentation(BsonType.String)]
        public UserRole Role { get; set; }

        [BsonElement("contact")]
        public string? Contact { get; set; }

        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;

        [BsonElement("enrolledCourses")]
        public List<string> EnrolledCourses { get; set; } = [];

        [BsonElement("teachingCourses")]
        public List<string> TeachingCourses { get; set; } = [];
    }

    public enum UserRole
    {
        Admin,
        User,
        Tutor
    }
}