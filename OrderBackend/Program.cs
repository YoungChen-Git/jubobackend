using Microsoft.EntityFrameworkCore;
using OrderBackend.Data;
using OrderBackend.Models;
using OrderBackend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure PostgreSQL Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 使用 Request/Response Logging Middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();

// Patient API Endpoints
// GET all patients
app.MapGet("/api/patients", async (ApplicationDbContext db) =>
{
    return await db.Patients.ToListAsync();
})
.WithName("GetAllPatients");

// GET patient by id
app.MapGet("/api/patients/{id}", async (string id, ApplicationDbContext db) =>
{
    var patient = await db.Patients.FindAsync(id);
    return patient is not null ? Results.Ok(patient) : Results.NotFound();
})
.WithName("GetPatientById");

// POST create new patient
app.MapPost("/api/patients", async (Patient patient, ApplicationDbContext db) =>
{
    db.Patients.Add(patient);
    await db.SaveChangesAsync();
    return Results.Created($"/api/patients/{patient.Id}", patient);
})
.WithName("CreatePatient");

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
.WithName("UpdatePatient");

// DELETE patient
app.MapDelete("/api/patients/{id}", async (string id, ApplicationDbContext db) =>
{
    var patient = await db.Patients.FindAsync(id);
    if (patient is null) return Results.NotFound();

    db.Patients.Remove(patient);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeletePatient");

// MedicalOrder API Endpoints
// GET all medical orders
app.MapGet("/api/medicalorders", async (ApplicationDbContext db) =>
{
    return await db.MedicalOrders.ToListAsync();
})
.WithName("GetAllMedicalOrders");

// GET medical order by id
app.MapGet("/api/medicalorders/{id}", async (string id, ApplicationDbContext db) =>
{
    var order = await db.MedicalOrders.FindAsync(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetMedicalOrderById");

// POST create new medical order
app.MapPost("/api/medicalorders", async (MedicalOrder order, ApplicationDbContext db) =>
{
    db.MedicalOrders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/medicalorders/{order.Id}", order);
})
.WithName("CreateMedicalOrder");

// PUT update medical order
app.MapPut("/api/medicalorders/{id}", async (string id, MedicalOrder updatedOrder, ApplicationDbContext db) =>
{
    var order = await db.MedicalOrders.FindAsync(id);
    if (order is null) return Results.NotFound();

    order.Message = updatedOrder.Message;

    await db.SaveChangesAsync();
    return Results.Ok(order);
})
.WithName("UpdateMedicalOrder");

// DELETE medical order
app.MapDelete("/api/medicalorders/{id}", async (string id, ApplicationDbContext db) =>
{
    var order = await db.MedicalOrders.FindAsync(id);
    if (order is null) return Results.NotFound();

    db.MedicalOrders.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteMedicalOrder");

app.Run();

