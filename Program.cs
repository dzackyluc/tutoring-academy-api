using DotNetEnv;
using Microsoft.Extensions.Options;
using TutoringAcademy.GraphQL;
using TutoringAcademy.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TutoringAcademy.Services;
using TutoringAcademy.GraphQL.Users;
using Amazon.S3;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure settings
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDBSettings"));
builder.Services.Configure<JWTSettings>(
    builder.Configuration.GetSection("JWTSettings"));
builder.Services.Configure<B2Settings>(
    builder.Configuration.GetSection("B2Settings"));

// Register services
builder.Services.AddSingleton<JWTService>();
builder.Services.AddSingleton(sp => {
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = new MongoDB.Driver.MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddSingleton<IAmazonS3>(sp => {
    var b2Settings = sp.GetRequiredService<IOptions<B2Settings>>().Value;
    return new AmazonS3Client(b2Settings.AccessKey, b2Settings.SecretKey, new AmazonS3Config
    {
        ServiceURL = b2Settings.BaseUrl,
        ForcePathStyle = true
    });
});

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

builder.Services.AddControllers();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<UserQueries>()
    .AddTypeExtension<UserMutations>()
    .AddAuthorization()
    .AddMongoDbFiltering()
    .AddMongoDbSorting()
    .AddMongoDbProjections()
    .AddMongoDbPagingProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL();

app.UseHttpsRedirection();

app.Run();
