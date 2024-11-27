using Microsoft.EntityFrameworkCore;
using ReelTrack.Helpers;
using ReelTrack.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ReelTrack.Endpoints;
using System.Security.Cryptography.X509Certificates;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.OpenApi;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenLocalhost(7235, listenOptions =>
    {
        var cert = X509Certificate2.CreateFromPemFile(
            "localhost+2.pem",
            "localhost+2-key.pem"
        );
        listenOptions.UseHttps(cert);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<ReelTrackDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var error = context.Exception;
                if (error is SecurityTokenExpiredException)
                    Console.WriteLine("Token has expired");
                else if (error is SecurityTokenInvalidSignatureException)
                    Console.WriteLine("Invalid token signature");
                else if (error is SecurityTokenInvalidIssuerException)
                    Console.WriteLine($"Invalid issuer. Expected: {options.TokenValidationParameters.ValidIssuer}");
                else if (error is SecurityTokenInvalidAudienceException)
                    Console.WriteLine($"Invalid audience. Expected: {options.TokenValidationParameters.ValidAudience}");

                Console.WriteLine($"Full error: {error}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    error = context.Error,
                    errorDescription = context.ErrorDescription,
                    token = context.Request.Headers["Authorization"].ToString(),
                    expectedIssuer = options.TokenValidationParameters.ValidIssuer,
                    expectedAudience = options.TokenValidationParameters.ValidAudience,
                    providedKey = options.TokenValidationParameters.IssuerSigningKey?.KeyId
                });
                return context.Response.WriteAsync(result);
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyOrigin()
                         .AllowAnyHeader()
                         .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowSpecificOrigin");

app.MapAuthEndpoints(app.Configuration);
app.MapFamilyEndpoints();

Console.WriteLine($"Current environment: {app.Environment.EnvironmentName}");

app.Run();
