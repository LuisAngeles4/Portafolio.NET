var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------
// REGISTRO DE SERVICIOS
// ----------------------------------------------------------
builder.Services.AddControllers()
    .AddNewtonsoftJson(opt =>
    {
        // Evita incluir nulos y corrige serialización de JObject/JValue
        opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------------
// CONSTRUCCIÓN DEL APP
// ----------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Si tu API corre solo con HTTP, puedes dejar esta línea comentada
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
