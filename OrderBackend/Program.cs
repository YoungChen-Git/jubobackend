using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderBackend.Data;
using OrderBackend.Models;
using OrderBackend.Middleware;
using OrderBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure PostgreSQL Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT Settings not found in configuration");

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddScoped<JwtTokenService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 使用 Request/Response Logging Middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

// Authentication API Endpoints
// Register new user
app.MapPost("/api/auth/register", async (
    ApplicationDbContext db,
    JwtTokenService jwtService,
    User newUser) =>
{
    // 檢查使用者名稱是否已存在
    if (await db.Users.AnyAsync(u => u.Username == newUser.Username))
    {
        return Results.BadRequest(new { message = "Username already exists" });
    }

    // 檢查 Email 是否已存在
    if (await db.Users.AnyAsync(u => u.Email == newUser.Email))
    {
        return Results.BadRequest(new { message = "Email already exists" });
    }

    // 建立新使用者
    var user = new User
    {
        Id = Guid.NewGuid().ToString(),
        Username = newUser.Username,
        Email = newUser.Email,
        PasswordHash = jwtService.HashPassword(newUser.PasswordHash), // PasswordHash 欄位在註冊時當作密碼使用
        CreatedAt = DateTime.UtcNow
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    // 生成 Token
    var token = jwtService.GenerateToken(user);

    return Results.Ok(new
    {
        message = "User registered successfully",
        token,
        user = new
        {
            user.Id,
            user.Username,
            user.Email
        }
    });
})
.WithName("Register");

// Login
app.MapPost("/api/auth/login", async (
    ApplicationDbContext db,
    JwtTokenService jwtService,
    LoginRequest loginRequest) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Username == loginRequest.Username);

    if (user == null || !jwtService.VerifyPassword(loginRequest.Password, user.PasswordHash))
    {
        return Results.Unauthorized();
    }

    var token = jwtService.GenerateToken(user);

    return Results.Ok(new
    {
        token,
        user = new
        {
            user.Id,
            user.Username,
            user.Email
        }
    });
})
.WithName("Login");

// Patient API Endpoints
// GET all patients
app.MapGet("/api/patients", async (ApplicationDbContext db) =>
{
    return await db.Patients.ToListAsync();
})
.WithName("GetAllPatients")
.RequireAuthorization();

// GET patient by id
app.MapGet("/api/patients/{id}", async (string id, ApplicationDbContext db) =>
{
    var patient = await db.Patients.FindAsync(id);
    return patient is not null ? Results.Ok(patient) : Results.NotFound();
})
.WithName("GetPatientById")
.RequireAuthorization();

// POST create new patient
app.MapPost("/api/patients", async (Patient patient, ApplicationDbContext db) =>
{
    db.Patients.Add(patient);
    await db.SaveChangesAsync();
    return Results.Created($"/api/patients/{patient.Id}", patient);
})
.WithName("CreatePatient")
.RequireAuthorization();

// PUT update patient
app.MapPut("/api/patients/{id}", async (string id, Patient updatedPatient, ApplicationDbContext db) =>
{
    var patient = await db.Patients.FindAsync(id);
    if (patient is null) return Results.NotFound();

    patient.Name = updatedPatient.Name;
    patient.OrderId = updatedPatient.OrderId;

    await db.SaveChangesAsync();
    return Results.Ok(patient);
})
.WithName("UpdatePatient")
.RequireAuthorization();

// DELETE patient
app.MapDelete("/api/patients/{id}", async (string id, ApplicationDbContext db) =>
{
    var patient = await db.Patients.FindAsync(id);
    if (patient is null) return Results.NotFound();

    db.Patients.Remove(patient);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeletePatient")
.RequireAuthorization();

// MedicalOrder API Endpoints
// GET all medical orders
app.MapGet("/api/medicalorders", async (ApplicationDbContext db) =>
{
    return await db.MedicalOrders.ToListAsync();
})
.WithName("GetAllMedicalOrders")
.RequireAuthorization();

// GET medical orders by patient id
app.MapGet("/api/patients/{patientId}/medicalorders", async (string patientId, ApplicationDbContext db) =>
{
    var orders = await db.MedicalOrders
        .Where(o => o.PatientId == patientId)
        .ToListAsync();
    
    return orders.Any() ? Results.Ok(orders) : Results.NotFound();
})
.WithName("GetMedicalOrdersByPatientId")
.RequireAuthorization();

// GET medical order by id
app.MapGet("/api/medicalorders/{id}", async (string id, ApplicationDbContext db) =>
{
    var order = await db.MedicalOrders.FindAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetMedicalOrderById")
.RequireAuthorization();

// POST create new medical order
app.MapPost("/api/medicalorders", async (MedicalOrder order, ApplicationDbContext db) =>
{
    db.MedicalOrders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/medicalorders/{order.Id}", order);
})
.WithName("CreateMedicalOrder")
.RequireAuthorization();

// PUT update medical order
app.MapPut("/api/medicalorders/{id}", async (string id, MedicalOrder updatedOrder, ApplicationDbContext db) =>
{
    var order = await db.MedicalOrders.FindAsync(id);
    if (order is null) return Results.NotFound();

    order.Message = updatedOrder.Message;

    await db.SaveChangesAsync();
    return Results.Ok(order);
})
.WithName("UpdateMedicalOrder")
.RequireAuthorization();

// DELETE medical order
app.MapDelete("/api/medicalorders/{id}", async (string id, ApplicationDbContext db) =>
{
    var order = await db.MedicalOrders.FindAsync(id);
    if (order is null) return Results.NotFound();

    db.MedicalOrders.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteMedicalOrder")
.RequireAuthorization();

app.Run();

