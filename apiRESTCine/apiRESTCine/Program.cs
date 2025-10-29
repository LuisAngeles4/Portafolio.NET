using apiRESTCine.Data; // acceso a CineDbContext y SucursalRepository

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1Configuración de servicios
// ------------------------------------------------------------

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controladores
builder.Services.AddControllers();

// Cadena de conexión (desde appsettings.json)
// Singleton: misma instancia para todo el proyecto
builder.Services.AddSingleton<CineDbContext>(sp =>
    new CineDbContext(builder.Configuration));

// Scoped: se crea una instancia nueva por cada petición HTTP
builder.Services.AddScoped<SucursalRepository>();

var app = builder.Build();

// ------------------------------------------------------------
// Configuración del pipeline HTTP
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Permitir HTTPS
// app.UseHttpsRedirection(); 

// Usar controladores (cine/sucursal/…)
app.MapControllers();

// ------------------------------------------------------------
// Endpoint de ejemplo (opcional, puedes eliminarlo luego)
// ------------------------------------------------------------
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// ------------------------------------------------------------
app.Run();

// ------------------------------------------------------------
//  Clase auxiliar (solo ejemplo)
// ------------------------------------------------------------
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
