using HotChocolate.Authorization;
using MongoDB.Driver;
using TutoringAcademy.DTOs;
using TutoringAcademy.Models;
using TutoringAcademy.Services;

namespace TutoringAcademy.GraphQL.Users
{
    [ExtendObjectType(typeof(Mutation))]
    public class UserMutations
    {
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

            var newUser = new User
            {
                Name = input.Name,
                Username = input.Username,
                Email = input.Email,
                Contact = input.Contact,
                Role = UserRole.User,
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
                Token = jwtService.GenerateToken(newUser)
            };
        }
    }
}