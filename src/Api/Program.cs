using Api.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Program>();

// configure logging
builder.ConfigureLogging();

// configure domain injections
builder.Services.AddDomainServices();

// configure infraestructure injections
builder.Services.AddInfraestructureServices(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddControllers()
    .AddFluentValidation(
        fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());    

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// configure middlewares
builder.Services.ConfigureMiddlewares();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// use middlewares
app.UseMiddlewares();

app.MapControllers();




app.Run();
