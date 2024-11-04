using Api.Extensions;
using Api.Middleware;
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
using Infrastructure.Notifications;
using Infrastructure.Notifications.QuartzJobs;
using Infrastructure.RepositoryImpl;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//Repository
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<ISpecialityRepository, SpecialityRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IIcdRepository, IcdRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IInspectionRepository, InspectionRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IDiagnosisRepository, DiagnosisRepository>();

//Services
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ISpecialityService, SpecialityService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IIcdService, IcdService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IDiagnosisService, DiagnosisService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IInspectionService, InspectionService>();

//Security
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddApiAuthentication(builder.Configuration);

//Validation
builder.Services.AddScoped<IValidator<DoctorLoginRequest>, DoctorLoginValidator>();
builder.Services.AddScoped<IValidator<DoctorRegistrationRequest>, DoctorRegistrationValidator>();
builder.Services.AddScoped<IValidator<DoctorEditRequest>, DoctorEditValidator>();
builder.Services.AddScoped<IValidator<PatientCreateRequest>, PatientCreateValidator>();
builder.Services.AddScoped<IValidator<InspectionCreateRequest>, InspectionCreateValidator>();
builder.Services.AddScoped<IValidator<ConsultationCreateRequest>, ConsultationCreateValidator>();
builder.Services.AddScoped<IValidator<InspectionCommentCreateRequest>, InspectionCommentCreateValidator>();
builder.Services.AddScoped<IValidator<DiagnosisCreateRequest>, DiagnosisCreateValidator>();
builder.Services.AddScoped<IValidator<CommentEditRequest>, CommentEditValidator>();
builder.Services.AddScoped<IValidator<ConsultationCommentCreateRequest>, ConsultationCommentCreateValidator>();
builder.Services.AddScoped<IValidator<InspectionEditRequest>, InspectionEditValidator>();

//Mapper
builder.Services.AddScoped<IDoctorMapper, DoctorMapper>();
builder.Services.AddScoped<ITokenMapper, TokenMapper>();
builder.Services.AddScoped<ISpecialityMapper, SpecialityMapper>();
builder.Services.AddScoped<IIcdMapper, IcdMapper>();
builder.Services.AddScoped<IPatientMapper, PatientMapper>();
builder.Services.AddScoped<IInspectionMapper, InspectionMapper>();
builder.Services.AddScoped<IDiagnosisMapper, DiagnosisMapper>();
builder.Services.AddScoped<ICommentMapper, CommentMapper>();
builder.Services.AddScoped<IConsultationMapper, ConsultationMapper>();
builder.Services.AddScoped<IReportMapper, ReportMapper>();

//Utilities
builder.Services.AddScoped<EmailSender>();
builder.Services.AddTransient<MissedInspectionsChecker>();
builder.Services.AddTransient<BannedTokenCleaner>();

//Quartz
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    q.AddJob<MissedInspectionsChecker>(options =>
    {
        options.WithIdentity("trigger1", "group1")
            .Build();
    });

    q.AddTrigger(options =>
    {
        options.ForJob("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x =>
                x.WithIntervalInHours(5)
                    .RepeatForever());
    });

    q.AddJob<BannedTokenCleaner>(options =>
    {
        options.WithIdentity("trigger2", "group2")
            .Build();
    });

    q.AddTrigger(options =>
    {
        options.ForJob("trigger2", "group2")
            .StartNow()
            .WithSimpleSchedule(x =>
                x.WithIntervalInHours(12)
                    .RepeatForever());
    });
});
builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var path = Path.Combine(AppContext.BaseDirectory, "Infrastructure", "Resources", "1.2.643.5.1.13.13.11.1005_2.27.json");

    try
    {
        IcdLoader.LoadIcd(context, path);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading ICD data: {ex.Message}");
    }
}


using (var scope = app.Services.CreateScope())
{
    var emailSender = scope.ServiceProvider.GetRequiredService<EmailSender>();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
