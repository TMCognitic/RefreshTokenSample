using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RefreshTokenSample.Api.Infrastructure;
using RefreshTokenSample.Api.Models.Entities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
JWTOptions options = configuration.GetSection("Api").Get<JWTOptions>()!;

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCqsHandlersAndDispatcher();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddSingleton<IList<User>>(sp => new List<User>()
{
    new User(Guid.NewGuid(), "Doe", "John", "john.doe@test.be", "Test1234=".Hash())
});

builder.Services.AddScoped(sp => options);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey))
        };
    });

builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();