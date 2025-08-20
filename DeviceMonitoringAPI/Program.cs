var builder = WebApplication.CreateBuilder(args);

// Добавить поддержку MVC контроллеров
builder.Services.AddControllers();

// Add services to the container.
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

// закомментил для избежания HTTPS redirect проблем. (пока по заданию не нужно)
// app.UseHttpsRedirection();

// Добавим поддержку routing контроллерам
app.UseRouting();
app.MapControllers();

app.Run();