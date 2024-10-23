using Application.Abstractions.Auth;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.BusinessLogic.Service;
using Application.BusinessLogic.Validation;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.RepositoryImpl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<ISpecialityRepository, SpecialityRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<DoctorLoginValidator, DoctorLoginValidator>();
builder.Services.AddScoped<DoctorRegistrationValidator, DoctorRegistrationValidator>();
builder.Services.AddScoped<DoctorEditValidator, DoctorEditValidator>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();
