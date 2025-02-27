using TodoApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity.Data;
using Task = TodoApi.Task;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
//     Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql")));

// var connectionString = builder.Configuration.GetConnectionString("ToDoDB") 
//                        ?? Environment.GetEnvironmentVariable("ToDoDB");

var envConnectionString = Environment.GetEnvironmentVariable("ToDoDB")?.Trim();
var connectionString = !string.IsNullOrEmpty(envConnectionString)
    ? envConnectionString
    : builder.Configuration.GetConnectionString("ToDoDB");


if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string for 'ToDoDB' is not set.");
}

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

Console.WriteLine($"Using Connection String: {connectionString}");

// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://fullstack-todolist-rcli.onrender.com") 
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var key = Encoding.ASCII.GetBytes("MySuperSecretKey1234567890123456111111111");

// var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "DefaultFallbackSecret";
// var key = Encoding.ASCII.GetBytes(jwtSecret);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"Connection String: {connectionString}");

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

// app.UseCors(policy =>
//     policy.AllowAnyOrigin()
//           .AllowAnyMethod()
//           .AllowAnyHeader());

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Thank you Tate!!");


app.MapGet("/tasks", async (ToDoDbContext context, HttpContext httpContext) =>
{
    var userId = GetUserIdFromToken(httpContext);
    if (userId == null) return Results.Unauthorized();

    var tasks = await context.Tasks.Where(t => t.UserId == userId).ToListAsync();
    return Results.Ok(tasks);
}).RequireAuthorization();


app.MapPost("/tasks", async (ToDoDbContext context, HttpContext httpContext, Task newTask) =>
{
    var userId = GetUserIdFromToken(httpContext);
    if (userId == null) return Results.Unauthorized();

    newTask.UserId = userId.Value;
    context.Tasks.Add(newTask);
    await context.SaveChangesAsync();

    return Results.Created($"/tasks/{newTask.Id}", newTask);
}).RequireAuthorization();


app.MapPut("/tasks/{id}", async (ToDoDbContext context, HttpContext httpContext, int id, Task updatedTask) =>
{
    var userId = GetUserIdFromToken(httpContext);
    if (userId == null) return Results.Unauthorized();

    var existingTask = await context.Tasks.FindAsync(id);
    if (existingTask == null || existingTask.UserId != userId) return Results.NotFound();

    existingTask.IsComplete = updatedTask.IsComplete;
    // existingTask.Name = updatedTask.Name;
    await context.SaveChangesAsync();

    return Results.Ok(existingTask);
}).RequireAuthorization();


app.MapDelete("/tasks/{id}", async (ToDoDbContext context, HttpContext httpContext, int id) =>
{
    var userId = GetUserIdFromToken(httpContext);
    if (userId == null) return Results.Unauthorized();

    var taskToDelete = await context.Tasks.FindAsync(id);
    if (taskToDelete == null || taskToDelete.UserId != userId) return Results.NotFound();

    context.Tasks.Remove(taskToDelete);
    await context.SaveChangesAsync();

    return Results.Ok();
}).RequireAuthorization();

app.MapPost("/register", async (ToDoDbContext context, User newUser) =>
{
    if (string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.PasswordHash))
        return Results.BadRequest("Username and Password are required");

    if (context.Users.Any(u => u.Username == newUser.Username))
        return Results.BadRequest("Username already exists");

    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);

    context.Users.Add(newUser);
    await context.SaveChangesAsync();

    return Results.Ok("User registered successfully");
});


// 🔹 התחברות והפקת טוקן עם ה- UserId
// app.MapPost("/login", async (ToDoDbContext context, LoginRequest request) =>
// {
//     var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
//     if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//         return Results.Unauthorized();

//     var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MySuperSecretKey1234567890123456111111111"));
//     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//     var claims = new[]
//     {
//         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//         new Claim(ClaimTypes.Name, user.Username)
//     };

//     var token = new JwtSecurityToken(
//         claims: claims,
//         expires: DateTime.UtcNow.AddHours(2),
//         signingCredentials: creds
//     );

//     var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

//     return Results.Ok(new { token = tokenString });
// });
app.MapPost("/login", async (ToDoDbContext context, LoginRequest request) =>
{
    try
    {
        var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Results.Unauthorized();

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MySuperSecretKey1234567890123456111111111"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new { token = tokenString });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in /login: {ex.Message}");
        return Results.Problem("An error occurred while processing your request.");
    }
});

app.Run();

// 🔹 פונקציה לשליפת UserId מהטוקן
int? GetUserIdFromToken(HttpContext httpContext)
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
}

