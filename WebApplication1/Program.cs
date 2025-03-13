using Data;
using Microsoft.EntityFrameworkCore;
using Services;
using Microsoft.OpenApi.Models;
using Models;
using Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Controller'ları ekle
builder.Services.AddEndpointsApiExplorer(); // API explorer'ı ekle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Manager API", Version = "v1" });
}); // Swagger'ı ekle

// Connection String'i `appsettings.json` içinden al
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext'i DI container'a ekle
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Repository ve Service Katmanlarını Tanımla
builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
//builder.Services.AddScoped<ISharePointService, SharePointService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Background Services
builder.Services.AddHostedService<BirthdayService>();

// HTTP Client ekle (SharePoint API çağrıları için)
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Manager API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
