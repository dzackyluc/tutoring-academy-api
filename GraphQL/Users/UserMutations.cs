using Amazon.S3;
using Amazon.S3.Model;
using HotChocolate.Authorization;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Users;
using TutoringAcademy.Models;
using TutoringAcademy.Services;

namespace TutoringAcademy.GraphQL.Users
{
    // This class defines GraphQL mutations for user-related operations such as login, registration, and profile updates. It includes authorization and file upload handling for profile pictures.
    [ExtendObjectType(typeof(Mutation))]
    public class UserMutations(IAmazonS3 s3Client)
    {
        // S3 client for handling profile picture uploads
        private readonly IAmazonS3 _s3Client = s3Client;

        // S3 bucket name for storing profile pictures
        private readonly string _bucketName = "tutoring-academy-bucket";

        // This mutation allows users to log in by providing their username and password. It returns a JWT token along with user information if the credentials are valid.
        [AllowAnonymous]
        public async Task<LoginResponse> LoginAsync(
            LoginInput input,
            [Service] IMongoDatabase database,
            [Service] JWTService jwtService)
        {
            var usersCollection = database.GetCollection<User>("users");
            var user = await usersCollection.Find(u => u.Username == input.Username).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash))
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Invalid username or password")
                    .SetCode("AUTH_ERROR")
                    .Build());
            }

            var token = jwtService.GenerateToken(user);

            return new LoginResponse
            {
                Token = token,
                Id = user.Id,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Username = user.Username,
                Email = user.Email
            };
        }

        // This mutation allows new users to register by providing their name, username, email, contact information, and password. It checks for existing usernames and emails to prevent duplicates and returns a JWT token along with user information upon successful registration.
        [AllowAnonymous]
        public async Task<RegisterResponse> RegisterAsync(
            RegisterInput input,
            [Service] IMongoDatabase database,
            [Service] JWTService jwtService)
        {
            var usersCollection = database.GetCollection<User>("users");

            if (await usersCollection.Find(u => u.Username == input.Username).AnyAsync())
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Username already exists")
                    .SetCode("USERNAME_EXISTS")
                    .Build());
            }

            if (await usersCollection.Find(u => u.Email == input.Email).AnyAsync())
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Email already exists")
                    .SetCode("EMAIL_EXISTS")
                    .Build());
            }

            var newUser = new User
            {
                Name = input.Name,
                Username = input.Username,
                Email = input.Email,
                Contact = input.Contact,
                Role = UserRole.User,
                AvatarUrl = "https://ui-avatars.com/api/?size=256&font-size=0.01&background=0D8ABC",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password)
            };

            await usersCollection.InsertOneAsync(newUser);

            return new RegisterResponse
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Username = newUser.Username,
                Email = newUser.Email,
                Contact = newUser.Contact,
                AvatarUrl = newUser.AvatarUrl,
                Token = jwtService.GenerateToken(newUser)
            };
        }

        // This mutation allows authenticated users to update their profile information, including name, email, contact information, and profile picture. It handles file uploads for the profile picture and updates the user's information in the database accordingly.
        [Authorize]
        public async Task<UpdateUserProfileResponse> UpdateUserProfileAsync(
            string userId,
            UpdateUserProfileInput input,
            [GraphQLType(typeof(UploadType))] IFile? profilePicture,
            [Service] IMongoDatabase database)
        {
            // Find the user in the database
            var usersCollection = database.GetCollection<User>("users");
             var user = await usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync() ?? throw new GraphQLException(
                    ErrorBuilder.New()
                    .SetMessage("User not found.")
                    .SetCode("USER_NOT_FOUND")
                    .Build());

            // Prepare the update definition for the user's profile information
            var updateDef = Builders<User>.Update
                .Set(u => u.Name, input.Name)
                .Set(u => u.Email, input.Email)
                .Set(u => u.Contact, input.Contact);

            // If a new profile picture is provided, upload it to S3 and update the AvatarUrl field
            if (profilePicture != null)
            {
                // Upload new profile picture to S3
                var imageKey = $"{userId}/{Guid.NewGuid()}_{profilePicture.Name}";
                var putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = imageKey,
                        InputStream = profilePicture.OpenReadStream(),
                        ContentType = profilePicture.ContentType
                    };
                    await _s3Client.PutObjectAsync(putRequest);
                user.AvatarUrl = $"https://storage.czn.my.id/{_bucketName}/{imageKey}";
            }

            // Update the user's profile information in the database
            var result = await usersCollection.UpdateOneAsync(u => u.Id == userId, updateDef);

            // If no documents were matched, throw an error indicating that the user was not found
            if (result.MatchedCount == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("User not found")
                    .SetCode("USER_NOT_FOUND")
                    .Build());
            }

            // Retrieve the updated user information from the database
            var updatedUser = await usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

            // Return the updated user profile information in the response
            return new UpdateUserProfileResponse
            {
                Id = updatedUser.Id,
                Name = updatedUser.Name,
                Username = updatedUser.Username,
                Email = updatedUser.Email,
                Contact = updatedUser.Contact,
                AvatarUrl = updatedUser.AvatarUrl
            };
        }
    }
}