global using shuttleasy.Services.UserServices;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using shuttleasy.Controllers;
using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.Companies;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.EFRepositories.Destinations;
using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.EFRepositories.ShuttleBuses;
using shuttleasy.DAL.EFRepositories.ShuttleSessions;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.Destinations;
using shuttleasy.LOGIC.Logics.PasswordReset;
using shuttleasy.LOGIC.Logics.ShuttleBuses;
using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.Mail;
using shuttleasy.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IPassengerLogic, PassengerLogic>();

builder.Services.AddScoped<ICompanyWorkerRepository, CompanyWorkerRepository>();
builder.Services.AddScoped<ICompanyWorkerLogic, CompanyWorkerLogic>();

builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<IPasswordResetLogic, PasswordResetLogic>();

builder.Services.AddScoped<IDestinationRepository, DestinationRepository>();
builder.Services.AddScoped<IDestinationLogic, DestinationLogic>();

builder.Services.AddScoped<IShuttleBusRepository, ShuttleBusRepository>();
builder.Services.AddScoped<IShuttleBusLogic, ShuttleBusLogic>();

builder.Services.AddScoped<IShuttleSessionRepository, ShuttleSessionRepository>();
builder.Services.AddScoped<IShuttleSessionLogic, ShuttleSessionLogic>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyLogic, CompanyLogic>();



builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<IMailManager, MailManager>();
builder.Services.AddScoped<IJwtTokenManager, JwtTokenManager>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordEncryption, PasswordEncryption>();
builder.Services.AddHttpContextAccessor();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme(\"bearer{token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
        });
});


builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value ?? throw new ArgumentNullException())),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


/*var hangfireConnectionString = "Server=.\\SQLSERVER;Database=DbCronJobHangfire;Trusted_Connection=true;";
builder.Services.AddHangfire(x =>
{
    x.UseSqlServerStorage(hangfireConnectionString);
    RecurringJob.AddOrUpdate<MailManager>(j => j.notifyPassengersPaymentDay(), "0 0 * * *");

});
builder.Services.AddHangfireServer();*/


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
//app.UseHangfireDashboard();
app.UseCors();

app.MapControllers();

app.Run();
