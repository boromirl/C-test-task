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
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200") // URL of Angular app
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Стандартный порядок:
// UseRouting -> UseCors -> UseAuthorization -> MapControllers/UseEndpoints

// Добавим поддержку routing контроллерам
app.UseRouting();

// Используем созданную CORS policy
// Важно, что должно быть после UseRouting() и до MapControllers()
app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();