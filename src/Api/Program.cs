using Api.Extensions;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Program>();

// configure logging
builder.ConfigureLogging();

// configure domain injections
builder.Services.AddDomainServices();

// configure infraestructure injections
builder.Services.AddInfraestructureServices(builder.Configuration);

// configure middlewares
builder.Services.ConfigureMiddlewares();


builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// use middlewares
app.UseMiddlewares();

app.Run();
