using DotNetEnv;
using Microsoft.Extensions.Options;
using TutoringAcademy.GraphQL;
using TutoringAcademy.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TutoringAcademy.Services;
using TutoringAcademy.GraphQL.Users;
using Amazon.S3;
using TutoringAcademy.GraphQL.Courses;
using TutoringAcademy.GraphQL.Reviews;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure settings from appsettings.json and environment variables
// The MongoDBSettings, JWTSettings, and B2Settings classes are configured to bind to their respective sections in the application configuration.
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.Configure<JWTSettings>(
    builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<B2Settings>(
    builder.Configuration.GetSection("B2Settings"));

// Register services for dependency injection
// The JWTService is registered as a singleton, allowing it to be injected into other parts of the application where JWT token generation is needed.
builder.Services.AddSingleton<JWTService>();
// The MongoDB client is configured using the connection string and database name from the MongoDBSettings, allowing the application to interact with the MongoDB database for data storage and retrieval.
builder.Services.AddSingleton(sp => {
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = new MongoDB.Driver.MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});
// The Amazon S3 client is configured using the B2Settings, allowing the application to interact with Backblaze B2 cloud storage for handling file uploads, such as profile pictures for users.
builder.Services.AddSingleton<IAmazonS3>(sp => {
    var b2Settings = sp.GetRequiredService<IOptions<B2Settings>>().Value;
    return new AmazonS3Client(b2Settings.AccessKey, b2Settings.SecretKey, new AmazonS3Config
    {
        ServiceURL = b2Settings.BaseUrl,
        ForcePathStyle = true
    });
});
// The authentication scheme is configured to use JWT Bearer tokens, allowing the application to authenticate users based on the JWT tokens they provide in their requests. 
// The token validation parameters are set based on the JWT settings defined in the application configuration, ensuring that the tokens are properly validated for issuer, audience, lifetime, and signing key.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JWTSettings").Get<JWTSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings!.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(jwtSettings!.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});
// CORS policy is configured to allow any origin, method, and header, enabling cross-origin requests from any client application that needs to interact with the API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
// The GraphQL server is configured with query and mutation types, as well as extensions for user-related operations. 
// It also includes authorization and MongoDB-specific features such as filtering, sorting, projections, and paging providers to enhance the functionality of the GraphQL API for the tutoring academy application.
builder.Services.AddControllers();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    // The DisableIntrospection method is called with a value of false, allowing clients to perform introspection queries on the GraphQL schema. This enables clients to explore the schema and understand the available types, queries, and mutations that they can interact with.
    .DisableIntrospection(false)
    // The UserQueries and UserMutations classes are added as type extensions to the GraphQL server, allowing them to define additional queries and mutations related to user operations such as login, registration, and profile management. 
    // This modular approach helps to organize the GraphQL schema and keep user-related logic separate from other parts of the application.
    .AddTypeExtension<UserQueries>()
    .AddTypeExtension<UserMutations>()
    // The CourseQueries and CourseMutations classes are added as type extensions to the GraphQL server, allowing them to define additional queries and mutations related to course operations such as creating, updating, and retrieving courses. 
    // This modular approach helps to organize the GraphQL schema and keep course-related logic separate from other parts of the application.
    .AddTypeExtension<CourseQueries>()
    .AddTypeExtension<CourseMutations>()
    // The ReviewQueries and ReviewMutations classes are added as type extensions to the GraphQL server, allowing them to define additional queries and mutations related to review operations such as creating, updating, and retrieving reviews. 
    // This modular approach helps to organize the GraphQL schema and keep review-related logic separate from other parts of the application.
    .AddTypeExtension<ReviewQueries>()
    .AddTypeExtension<ReviewMutations>()
    // Authorization is added to the GraphQL server, enabling the use of authorization attributes on queries and mutations to restrict access based on user roles and authentication status.
    .AddAuthorization()
    // MongoDB-specific features are added to the GraphQL server, allowing for filtering, sorting, projections, and paging of data retrieved from MongoDB collections in response to GraphQL queries.
    .AddMongoDbFiltering()
    // The AddMongoDbSorting method is added to enable sorting of data retrieved from MongoDB collections in response to GraphQL queries, allowing clients to specify sorting criteria for the results.
    .AddMongoDbSorting()
    // The AddMongoDbProjections method is added to enable projections of data retrieved from MongoDB collections in response to GraphQL queries, allowing clients to specify which fields to include or exclude in the results.
    .AddMongoDbProjections()
    // The AddMongoDbPagingProviders method is added to enable paging of data retrieved from MongoDB collections in response to GraphQL queries, allowing clients to specify pagination parameters such as page size and page number for the results.
    .AddMongoDbPagingProviders();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
// The authentication and authorization middleware are added to the request pipeline, ensuring that incoming requests are properly authenticated and authorized based on the JWT tokens provided by the clients.
app.UseAuthentication();
app.UseAuthorization();

// The controllers and GraphQL endpoints are mapped to the request pipeline, allowing the application to handle incoming HTTP requests for both RESTful API endpoints defined in controllers and GraphQL queries and mutations defined in the GraphQL schema.
app.MapControllers();
app.MapGraphQL();

// The HTTPS redirection middleware is added to the request pipeline, ensuring that all incoming requests are redirected to use HTTPS for secure communication between clients and the server.
app.UseHttpsRedirection();
// The CORS middleware is added to the request pipeline with the "AllowAll" policy, enabling cross-origin requests from any client application that needs to interact with the API.
app.UseCors("AllowAll");


// The application is run, starting the web server and allowing it to listen for incoming requests on the configured port.
app.Run();
