using Application.Abstractions.Auth;
using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.BusinessLogic.Mapper;
using Application.BusinessLogic.Service;
using Application.BusinessLogic.Validation;
using Application.Dto;
using FluentValidation;
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
builder.Services.AddScoped<IValidator<DoctorLoginRequest>, DoctorLoginValidator>();
builder.Services.AddScoped<IValidator<DoctorRegistrationRequest>, DoctorRegistrationValidator>();
builder.Services.AddScoped<IValidator<DoctorEditRequest>, DoctorEditValidator>();
builder.Services.AddScoped<IDoctorMapper, DoctorMapper>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ITokenMapper, TokenMapper>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
