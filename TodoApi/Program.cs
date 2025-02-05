// using TodoApi;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.IdentityModel.Tokens;
// using System.Text;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using BCrypt.Net;
// using Microsoft.AspNetCore.Identity.Data;


// var builder = WebApplication.CreateBuilder(args);

// //הזרקת תלויות
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
//     Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql")));

// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.AllowAnyOrigin()
//               .AllowAnyMethod()
//               .AllowAnyHeader();
//     });
// });

// // הוספת שירותי אימות עם JWT

// var key = Encoding.ASCII.GetBytes("MySuperSecretKey1234567890123456"); // לפחות 32 תווים
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(key),
//             ValidateIssuer = false,
//             ValidateAudience = false
//         };
//     });//הוספת שירותי הרשאות כדי למנוע שגיאה
// builder.Services.AddAuthorization();

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// app.UseCors(policy =>
//     policy.AllowAnyOrigin()
//           .AllowAnyMethod()
//           .AllowAnyHeader());

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// // הפעלת אימות ואוטוריזציה
// app.UseAuthentication();
// app.UseAuthorization();

// // הגדרת ה-Routes
// app.MapGet("/", () => "Thank you Tate!!");

// // app.MapGet("/tasks", async (ToDoDbContext context) =>
// // {
// //     return await context.Tasks.ToListAsync();
// // }).RequireAuthorization();

// app.MapGet("/tasks", async (ToDoDbContext context, HttpContext httpContext) =>
// {
//     var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//     if (userIdClaim == null)
//     {
//         return Results.Unauthorized();
//     }

//     int userId = int.Parse(userIdClaim.Value);
//     var tasks = await context.Tasks.Where(t => t.UserId == userId).ToListAsync();
//     return Results.Ok(tasks);
// }).RequireAuthorization();

// // app.MapPost("/tasks", async (ToDoDbContext context, ToDoTask newTask) =>
// // {
// //     context.Tasks.Add(newTask);
// //     await context.SaveChangesAsync();
// //     return Results.Created($"/tasks/{newTask.Id}", newTask);
// // }).RequireAuthorization();

// app.MapPost("/tasks", async (ToDoDbContext context, HttpContext httpContext, ToDoTask newTask) =>
// {
//     var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
//     if (userIdClaim == null)
//     {
//         return Results.Unauthorized();
//     }

//     newTask.UserId = int.Parse(userIdClaim.Value);
//     context.Tasks.Add(newTask);
//     await context.SaveChangesAsync();

//     return Results.Created($"/tasks/{newTask.Id}", newTask);
// }).RequireAuthorization();

// app.MapPut("/tasks/{id}", async (ToDoDbContext context, int id, ToDoTask updatedItem) =>
// {
//     var existingTask= await context.Tasks.FindAsync(id);
//     if (existingTask != null)
//     {
//         existingTask.IsComplete = updatedItem.IsComplete;
//         await context.SaveChangesAsync();
//         return Results.Ok(existingTask);
//     }
//     return Results.NotFound();
// }).RequireAuthorization();


// app.MapDelete("/tasks/{id}", async (ToDoDbContext context, int id) =>
// {
//     var itemToDelete = await context.Tasks.FindAsync(id);
//     if (itemToDelete != null)
//     {
//         context.Tasks.Remove(itemToDelete);
//         await context.SaveChangesAsync();
//         return Results.Ok();
//     }
//     return Results.NotFound();
// }).RequireAuthorization();


// app.MapPost("/register", async (ToDoDbContext context, User newUser) =>
// {
//     // בדיקה אם המשתמש קיים
//     if (context.Users.Any(u => u.Username == newUser.Username))
//         return Results.BadRequest("Username already exists");

//     // הצפנת הסיסמה
//     newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
    
//     context.Users.Add(newUser);
//     await context.SaveChangesAsync();
    
//     return Results.Ok("User registered successfully");
// });

// // app.MapPost("/login", async (ToDoDbContext context, LoginRequest request) =>
// // {
// //     var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
// //     if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
// //         return Results.Unauthorized();

// //     // יצירת טוקן JWT
// //     var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MySuperSecretKeyThatIsLongEnough123!"));
// //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

// //     var token = new JwtSecurityToken(
// //         claims: new[] { new Claim(ClaimTypes.Name, user.Username) },
// //         expires: DateTime.UtcNow.AddHours(2),
// //         signingCredentials: creds
// //     );

// //     var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
// //     Console.WriteLine("🔹 נוצר טוקן חדש: " + tokenString);

// //     return Results.Ok(new { token = tokenString });
// // });
// app.MapPost("/login", async (ToDoDbContext context, LoginRequest request) =>
// {
//     var user = context.Users.FirstOrDefault(u => u.Username == request.Username);
//     if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
//         return Results.Unauthorized();

//     var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("MySuperSecretKey1234567890123456"));
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

// app.Run();


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

// 🔹 הזרקת תלויות
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql")));

// 🔹 הגדרת CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 🔹 מפתח ה-JWT (ודא שזה זהה גם ביצירת הטוקן)
var key = Encoding.ASCII.GetBytes("MySuperSecretKey1234567890123456111111111"); 

// 🔹 הוספת שירותי אימות ואוטוריזציה
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

var app = builder.Build();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 הפעלת אימות ואוטוריזציה
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


app.MapPut("/tasks/{id}", async (ToDoDbContext context, HttpContext httpContext, int id, bool isComplete) =>
{
    var userId = GetUserIdFromToken(httpContext);
    if (userId == null) return Results.Unauthorized();

    var existingTask = await context.Tasks.FindAsync(id);
    if (existingTask == null || existingTask.UserId != userId) return Results.NotFound();

    existingTask.IsComplete = isComplete;
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
app.MapPost("/login", async (ToDoDbContext context, LoginRequest request) =>
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
});

app.Run();

// 🔹 פונקציה לשליפת UserId מהטוקן
int? GetUserIdFromToken(HttpContext httpContext)
{
    var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
}

