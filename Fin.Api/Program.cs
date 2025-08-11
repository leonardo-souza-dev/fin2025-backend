using Fin.Api.Data;
using Fin.Api.Infra;
using Fin.Api.Repository;
using Fin.Api.Services;
using Fin.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace Fin.Api;

public class Program
{
    private const string FIN2025_JWT_SECRET_KEY = "FIN2025_JWT_SECRET_KEY";
    private const string FIN2025_DATABASE_CONNECTION = "FIN2025_DATABASE_CONNECTION";

    private static void AddApplicationDependencies(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ServerPortInfraService>();

        var connectionString = Environment.GetEnvironmentVariable(FIN2025_DATABASE_CONNECTION);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "String de conexão não configurada. Configure a variável de ambiente FIN2025_DATABASE_CONNECTION ou a configuração DefaultConnection.");            
        }

        builder.Services.AddDbContext<FinDbContext>(options => options.UseNpgsql(connectionString));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ITransferRepository, TransferRepository>();

        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<ConfigService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<MonthService>();

        builder.Services.AddScoped<TransactionService>();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        
        var secretKey = Environment.GetEnvironmentVariable(FIN2025_JWT_SECRET_KEY);

        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException(
                "JWT Secret Key não configurada. Configure a variável de ambiente FIN2025_JWT_SECRET_KEY ou use User Secrets.");
        }

        var secretKeyBytes = Encoding.ASCII.GetBytes(secretKey);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero // Remove tolerância de tempo
            };
        });
        builder.Services.AddAuthorization();


        var myAllowSpecificOrigins = "_loremIpsumD";

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = Assembly.GetExecutingAssembly().GetName().Name, Version = "v0" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."

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
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: myAllowSpecificOrigins,
                              policy =>
                              {
                                  policy
                                    .WithOrigins(
                                      "http://127.0.0.1:3000", 
                                      "https://127.0.0.1:3000",
                                      "http://localhost:3000",
                                      "https://localhost:3000",
                                      "http://[::1]:3000",
                                      "https://[::1]:3000")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowCredentials();
                              });
        });
        builder.Services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        AddApplicationDependencies(builder);

        var app = builder.Build();

        app.UseCors(myAllowSpecificOrigins);// before useAuthentication, and useAuthorization
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}