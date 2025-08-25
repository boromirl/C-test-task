using Microsoft.OpenApi.Models;
using System.Reflection;    // XML comments
using Serilog;
using DeviceMonitoringAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавить поддержку MVC контроллеров
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавим CORS, чтобы не было "cross-origin"
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200", "http://angular:80") // URL of Angular app
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Документация Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Device Monitoring API",
        Version = "v1",
        Description = "API for monitoring device activities"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IBackupService, BackupService>();

// Конфигурация Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
       c.SwaggerEndpoint("../swagger/v1/swagger.json", "Device Monitoring API v1");
    });
}

// не используем https в тестовом задании
// app.UseHttpsRedirection();

// Стандартный порядок:
// UseRouting -> UseCors -> UseAuthorization -> MapControllers/UseEndpoints

// Добавим поддержку routing контроллерам
app.UseRouting();

// Используем созданную CORS policy
// Важно, что должно быть после UseRouting() и до MapControllers()
app.UseCors("AllowAngularApp");

app.MapControllers();

// Логгинг всех входящих реквестов
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Handling request: {Method} {Path}", context.Request.Method, context.Request.Path);

    await next();

    logger.LogInformation("Finished handling request. Status: {StatusCode}", context.Response.StatusCode);
});

app.Run();