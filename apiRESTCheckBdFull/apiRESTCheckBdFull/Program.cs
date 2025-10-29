var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();        // habilita controladores
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();                     // mapea CheckBdController y UsuariosController

app.Run();
