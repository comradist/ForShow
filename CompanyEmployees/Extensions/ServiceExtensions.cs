using Contracts;
using Repository;
using LoggerService;
using Services;
using Services.Contracts;
using Microsoft.EntityFrameworkCore;
using CompanyEmployees.ContentNegotiation;
using CompanyEmployees.Presentation.ActionFilters;
using Services.DataShaping;
using Shared.DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace CompanyEmployees.Extensions;

public static class ServiceExtensions
{
    public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

    public static void ConfigureIdentity(this IServiceCollection service)
    {
        var builder = service.AddIdentity<User, IdentityRole>(role => {
            role.Password.RequireDigit = true;
            role.Password.RequireLowercase = false;
            role.Password.RequireUppercase = false;
            role.Password.RequireNonAlphanumeric = false;
            role.Password.RequiredLength = 10;
            role.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<RepositoryContext>()
        .AddDefaultTokenProviders();
    }

    public static void AddCustomMediaTypes(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(config =>
        {
            var systemTextJsonOutputFormatter = config.OutputFormatters.OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

            if (systemTextJsonOutputFormatter != null)
            {
                systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/hateoas+json");
                systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/apiroot+json");
            }
        });
    }

    public static void ConfigureRateLimitingOption(this IServiceCollection services)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = 30,
                Period = "5m",
            }
        };

        services.Configure<IpRateLimitOptions>(opt => { opt.GeneralRules = rateLimitRules; });
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });
    }

    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("X-Pagination"));
        });

    public static void ConfigureIISIntegration(this IServiceCollection service) =>
        service.Configure<IISOptions>(options =>
        {
        });

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext<RepositoryContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("sqlConnection")));

    public static void ConfigureValidationMediaTypeAttribute(this IServiceCollection services) =>
        services.AddScoped<ValidateMediaTypeAttribute>();

    public static void ConfigureValidationFilterAttribute(this IServiceCollection services) =>
        services.AddScoped<ValidationFilterAttribute>();

    public static void ConfigureDataShaper(this IServiceCollection services) =>
        services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

    public static void ConfigureResponseCache(this IServiceCollection services) =>
        services.AddResponseCaching();

    public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
        services.AddHttpCacheHeaders(
            (expOpt) => 
            {
                expOpt.MaxAge = 65;
                expOpt.CacheLocation = CacheLocation.Private;
            },
            (validOpt) =>
            {
                validOpt.MustRevalidate = true;
            });
}