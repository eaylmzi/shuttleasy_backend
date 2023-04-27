global using shuttleasy.Services.UserServices;
using CorePush.Apple;
using CorePush.Google;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using shuttleasy.Controllers;
using shuttleasy.DAL.EFRepositories;
using shuttleasy.DAL.EFRepositories.Companies;
using shuttleasy.DAL.EFRepositories.CompanyWorkers;
using shuttleasy.DAL.EFRepositories.DriversStatistics;
using shuttleasy.DAL.EFRepositories.GeoPoints;
using shuttleasy.DAL.EFRepositories.NotificationPassengers;
using shuttleasy.DAL.EFRepositories.NotificationWorkers;
using shuttleasy.DAL.EFRepositories.PassengerRatings;
using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.EFRepositories.PickupAreas;
using shuttleasy.DAL.EFRepositories.PickupPoints;
using shuttleasy.DAL.EFRepositories.SessionHistories;
using shuttleasy.DAL.EFRepositories.SessionPassengers;
using shuttleasy.DAL.EFRepositories.ShuttleBuses;
using shuttleasy.DAL.EFRepositories.ShuttleSessions;
using shuttleasy.DAL.EFRepositories.ShuttleSessionSearch;
using shuttleasy.Encryption;
using shuttleasy.JwtToken;
using shuttleasy.LOGIC.Logics;
using shuttleasy.LOGIC.Logics.Companies;
using shuttleasy.LOGIC.Logics.CompanyWorkers;
using shuttleasy.LOGIC.Logics.DriversStatistics;
using shuttleasy.LOGIC.Logics.GeoPoints;
using shuttleasy.LOGIC.Logics.JoinTables;
using shuttleasy.LOGIC.Logics.NotificationPassengers;
using shuttleasy.LOGIC.Logics.NotificationWorkers;
using shuttleasy.LOGIC.Logics.PassengerRatings;
using shuttleasy.LOGIC.Logics.PasswordReset;
using shuttleasy.LOGIC.Logics.PickupAreas;
using shuttleasy.LOGIC.Logics.PickupPoints;
using shuttleasy.LOGIC.Logics.SessionHistories;
using shuttleasy.LOGIC.Logics.SessionPassengers;
using shuttleasy.LOGIC.Logics.ShuttleBuses;

using shuttleasy.LOGIC.Logics.ShuttleSessions;
using shuttleasy.Mail;
using shuttleasy.Services;
using shuttleasy.Services.NotifService;
using shuttleasy.Services.ShuttleServices;
using shuttleasy.Services.SocketServices;
using Swashbuckle.AspNetCore.Filters;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IPassengerLogic, PassengerLogic>();

builder.Services.AddScoped<ICompanyWorkerRepository, CompanyWorkerRepository>();
builder.Services.AddScoped<ICompanyWorkerLogic, CompanyWorkerLogic>();

builder.Services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
builder.Services.AddScoped<IPasswordResetLogic, PasswordResetLogic>();

builder.Services.AddScoped<IShuttleBusRepository, ShuttleBusRepository>();
builder.Services.AddScoped<IShuttleBusLogic, ShuttleBusLogic>();

builder.Services.AddScoped<IShuttleSessionRepository, ShuttleSessionRepository>();
builder.Services.AddScoped<IShuttleSessionLogic, ShuttleSessionLogic>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyLogic, CompanyLogic>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyLogic, CompanyLogic>();

builder.Services.AddScoped<INotificationPassengerRepository, NotificationPassengerRepository>();
builder.Services.AddScoped<INotificationPassengerLogic, NotificationPassengerLogic>();

builder.Services.AddScoped<INotificationWorkerRepository, NotificationWorkerRepository>();
builder.Services.AddScoped<INotificationWorkerLogic, NotificationWorkerLogic>();

builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IPassengerLogic, PassengerLogic>();

builder.Services.AddScoped<IPassengerRatingRepository, PassengerRatingRepository>();
builder.Services.AddScoped<IPassengerRatingLogic, PassengerRatingLogic>();

builder.Services.AddScoped<ISessionHistoryRepository, SessionHistoryRepository>();
builder.Services.AddScoped<ISessionHistoryLogic, SessionHistoryLogic>();

builder.Services.AddScoped<ISessionPassengerRepository, SessionPassengerRepository>();
builder.Services.AddScoped<ISessionPassengerLogic, SessionPassengerLogic>();

builder.Services.AddScoped<IDriversStatisticRepository, DriversStatisticRepository>();
builder.Services.AddScoped<IDriversStatisticLogic, DriversStatisticLogic>();

builder.Services.AddScoped<IPickupAreaRepository, PickupAreaRepository>();
builder.Services.AddScoped<IPickupAreaLogic, PickupAreaLogic>();

builder.Services.AddScoped<IPickupPointRepository, PickupPointRepository>();
builder.Services.AddScoped<IPickupPointLogic, PickupPointLogic>();

builder.Services.AddScoped<IJoinTableLogic, JoinTableLogic>();

builder.Services.AddScoped<IGeoPointLogic, GeoPointLogic>();
builder.Services.AddScoped<IGeoPointRepository, GeoPointRepository>();


builder.Services.AddSignalR();



builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<IMailManager, MailManager>();
builder.Services.AddScoped<IJwtTokenManager, JwtTokenManager>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordEncryption, PasswordEncryption>();
builder.Services.AddScoped<IShuttleService, ShuttleService>();
builder.Services.AddTransient<INotificationService, NotificationService>();
builder.Services.AddScoped<ISocketService, SocketService>();
builder.Services.AddHttpClient<FcmSender>();
builder.Services.AddHttpClient<ApnSender>();
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

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("x-api-version"),
                                                    new MediaTypeApiVersionReader("x-api-version"));
});
builder.Services.AddControllers();
builder.Services.AddApiVersioning();
builder.Services.Configure<ApiVersioningOptions>(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
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
//WEB SOCKET OPTIONS
/*
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(3)
};
app.UseWebSockets(webSocketOptions);
*/
/*
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await SocketService Echo(context, webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});
*/
// Configure the HTTP request pipeline.
app.UseRouting();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();
//app.UseHangfireDashboard();
app.UseCors();

app.MapControllers();

app.Run();
