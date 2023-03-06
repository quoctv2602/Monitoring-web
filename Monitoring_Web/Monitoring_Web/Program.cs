using Microsoft.EntityFrameworkCore;
using Monitoring.Data;
using Microsoft.Extensions.Configuration;
using Monitoring.Service;
using Monitoring.Service.IService;
using Monitoring.Data.IRepository;
using Monitoring.Data.Repository;
using DocumentFormat.OpenXml.InkML;
using Monitoring.Service.Services;
using Monitoring_Web.HubConfig;
using Monitoring_Web.TimerFeatures;
using Microsoft.AspNetCore.SignalR;
using Monitoring_Web.Helpers;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Monitoring_Web.Filter;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using static System.Net.WebRequestMethods;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.
        ConfiguringService(builder);
        builder.Logging.ClearProviders();
        builder.Logging.AddLog4Net();

        builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
        builder.Services.AddSwaggerGen();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors(x => x.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("https://localhost:4200", "http://localhost:4200", "http://localhost:4300", "https://localhost:4300", "https://localhost:44329", "https://localhost:44429")
            .WithExposedHeaders("Content-Disposition")
            );

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapHub<ChartHub>("/api/chart");

        app.MapControllerRoute(
            name: "default",
            pattern: "api/{controller}/{action=Index}/{id?}");

        app.MapFallbackToFile("index.html"); ;
        app.UseSession();
        app.Run();
    }
    public static void ConfiguringService(WebApplicationBuilder builder)
    {
        var services = builder.Services;

        //services.AddDataProtection()
        //.UseCryptographicAlgorithms(new Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorConfiguration()
        //{
        //    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
        //    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        //});

        //var configManager = new ConfigurationManager<OpenIdConnectConfiguration>("https://login.microsoftonline.com/common/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
        //var openidconfig = configManager.GetConfigurationAsync().Result;
        var clientId = builder.Configuration.GetSection("IDP")["ClientId"];
        var authority = builder.Configuration.GetSection("IDP")["Authority"];
        var slidingExpiration = builder.Configuration.GetValue<int>("SessionTimeout");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ValidAudience = clientId,
            };
            options.MetadataAddress = authority + ".well-known/openid-configuration";
        });

        services.AddSignalR(o =>
        {
            o.EnableDetailedErrors = true;
        });
        services.AddControllersWithViews();

        services.AddDbContext<MonitoringContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("MonitoringTool"));
        }
        );
        services.AddSession();
        services.AddDistributedSqlServerCache(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString(
                "MonitoringTool");
            options.SchemaName = "dbo";
            options.TableName = "Sys_Monitoring_Session";
            options.DefaultSlidingExpiration = TimeSpan.FromMinutes(slidingExpiration);
        });
        services.AddSingleton<TimerManager>();
        //--------------------------------------------------------------------------------
        services.AddScoped<INodeSettingService, NodeSettingService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IIntegrationAPIService, IntegrationAPIService>();
        services.AddScoped<ITransactionBaseService, TransactionBaseService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAccountService, AccountService>();
        //--------------------------------------------------------------------------------
        services.AddScoped<INodeSettingRepository, NodeSettingRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IIntegrationAPIRepository, IntegrationAPIRepository>();
        services.AddScoped<ITransactionBaseRepository, TransactionBaseRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IReprocessRepository, ReprocessRepository>();
    }
}