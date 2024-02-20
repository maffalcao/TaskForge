using Api.Middlewares;
using Api.Validators;
using Domain.Dtos;
using Domain.Interfaces.Persistence;
using Domain.Interfaces.Services;
using Domain.Services;
using FluentValidation;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);

// log configuration
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Minute)
);

// api registration
builder.Services.AddTransient<IValidator<AddProjectDto>, AddProjectDtoValidator>();


// infraestructure registrations

builder.Services.AddDbContext<ApplicationContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskForgeDbConnectionString"))
);
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// application registrations
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ValidateUserMiddleware>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestResponseLoggingMiddleware>();

builder.Services.AddSingleton<Program>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// middlewares
app.UseMiddleware<ValidateUserMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.Run();
