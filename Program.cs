using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TechnicalTestApi.Models;
using TechnicalTestApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration to allow requests from a React app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")  // Allow requests from the React app running on localhost:3000
                   .AllowAnyMethod()  // Allow all HTTP methods (GET, POST, etc.)
                   .AllowAnyHeader()  // Allow all headers in the requests
                   .AllowCredentials();  // Allow credentials
        });
});

// MongoDB configuration
builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection(nameof(MongoDBSettings)));

// Register UserService and NoteService as singleton services
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<NoteService>();

// JWT configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  // Set default authentication scheme to JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;  // Set default challenge scheme to JWT Bearer
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,  // Validate the token issuer
        ValidateAudience = true,  // Validate the token audience
        ValidateLifetime = true,  // Validate the token expiration
        ValidateIssuerSigningKey = true,  // Validate the signing key used to generate the token
        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Set the valid issuer from configuration
        ValidAudience = builder.Configuration["Jwt:Audience"],  // Set the valid audience from configuration
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))  // Set the signing key from configuration
    };
});

// Add controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();  
}

app.UseStaticFiles();  // Serve static files (e.g., HTML, CSS, JS)
app.UseRouting();  // Enable routing for the application

// Use the configured CORS policy
app.UseCors("AllowReactApp");

// Use authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

// Map fallback to index.html for SPA (Single Page Application)
app.MapFallbackToFile("index.html");

app.Run();  // Run the application
