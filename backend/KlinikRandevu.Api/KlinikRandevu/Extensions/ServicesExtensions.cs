
using Entities.Exeptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Contracts;
using Repositories.EFCore;
using Serilog;
using Services;
using Services.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.RateLimiting;

namespace KlinikRandevu.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                m => m.MigrationsAssembly("KlinikRandevu")
                ));
        }
        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }
        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IUserYetkiService, UserYetkiManager>();
            //  services.AddScoped<IEmailService, EmailManager>();
            services.AddHttpClient();
        }

        public static void UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionMiddleware>();
        }
        public static void AddSerilogLogging(this WebApplicationBuilder builder)
        {
            Log.Logger= new LoggerConfiguration().WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Debug().CreateLogger(); // prod ortamında info'ya geçirilcek
            builder.Host.UseSerilog();
        }
        public static IServiceCollection CorsConfigure(this IServiceCollection services) // prod ortamında kısıtlama getirilcek
        {
            services.AddCors(options=>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            return services;
        }

        public static IServiceCollection ConfigureRateLimiter(this IServiceCollection services)
        {
            //services.AddRateLimiter(options =>
            //{
            //    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            //    options.AddFixedWindowLimiter(policyName: "RateLimit", opt =>
            //    {
            //        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString()
            //            ?? "unknown";
            //        opt.PermitLimit = 100;
            //        opt.Window = TimeSpan.FromSeconds(30);
            //        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //        opt.QueueLimit = 3;
            //    });
            //});
            //return services;

            services.AddRateLimiter(options=>
            {
                options.RejectionStatusCode=StatusCodes.Status429TooManyRequests;
                options.AddPolicy("RateLimit", httpContext =>
                {
                    var ipAdress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ipAdress,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit=100,
                            Window=TimeSpan.FromSeconds(30),
                            QueueProcessingOrder=QueueProcessingOrder.OldestFirst,
                            QueueLimit=3
                        }
                        );
                });
            });
            return services;
        }

        public static IServiceCollection ConfigureJWTToken(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options=>
            {
                options.TokenValidationParameters= new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["jwt:Issuer"],
                    ValidAudience = configuration["jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["jwt:Key"])
            )
                };
            });
            return services;
        }
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
            });

            return services;
        }
    }
}
