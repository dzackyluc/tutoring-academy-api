using Amazon.S3;
using Amazon.S3.Model;
using HotChocolate.Authorization;
using MongoDB.Driver;
using TutoringAcademy.DTOs.Users;
using TutoringAcademy.Models;
using TutoringAcademy.Services;

namespace TutoringAcademy.GraphQL.Users
{
    [ExtendObjectType(typeof(Mutation))]
    public class UserMutations(IAmazonS3 s3Client)
    {
        private readonly IAmazonS3 _s3Client = s3Client;

        private readonly string _bucketName = "tutoring-academy-bucket";

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

        [Authorize]
        public async Task<UpdateUserProfileResponse> UpdateUserProfileAsync(
            string userId,
            UpdateUserProfileInput input,
            [GraphQLType(typeof(UploadType))] IFile? profilePicture,
            [Service] IMongoDatabase database)
        {
            var usersCollection = database.GetCollection<User>("users");
             var user = await usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync() ?? throw new GraphQLException(
                    ErrorBuilder.New()
                    .SetMessage("User not found.")
                    .SetCode("USER_NOT_FOUND")
                    .Build());

            var updateDef = Builders<User>.Update
                .Set(u => u.Name, input.Name)
                .Set(u => u.Email, input.Email)
                .Set(u => u.Contact, input.Contact);

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

            var result = await usersCollection.UpdateOneAsync(u => u.Id == userId, updateDef);

            if (result.MatchedCount == 0)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("User not found")
                    .SetCode("USER_NOT_FOUND")
                    .Build());
            }

            var updatedUser = await usersCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();

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