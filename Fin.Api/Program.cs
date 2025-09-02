using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Fin.Application.UseCases;
using Fin.Application.UseCases.Accounts;
using Fin.Application.UseCases.Auth;
using Fin.Application.UseCases.Banks;
using Fin.Application.UseCases.Configs;
using Fin.Application.UseCases.Months;
using Fin.Application.UseCases.Payments;
using Fin.Application.UseCases.Transfers;
using Fin.Infrastructure.Data;
using Fin.Infrastructure.Repositories;

namespace Fin.Api;

public class Program
{
    private const string FIN2025_JWT_SECRET_KEY = "FIN2025_JWT_SECRET_KEY";
    private const string FIN2025_DATABASE_CONNECTION = "FIN2025_DATABASE_CONNECTION";

    private static void AddApplicationDependencies(WebApplicationBuilder builder)
    {
        var connectionString = Environment.GetEnvironmentVariable(FIN2025_DATABASE_CONNECTION);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "String de conexão não configurada. Configure a variável de ambiente FIN2025_DATABASE_CONNECTION ou a configuração DefaultConnection.");            
        }

        builder.Services.AddDbContext<FinDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Scoped);

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IBankRepository, BankRepository>();
        builder.Services.AddScoped<IConfigRepository, ConfigRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ITransferRepository, TransferRepository>();
        builder.Services.AddScoped<IRecurrenceRepository, RecurrenceRepository>();
        
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ConfigService>();
        builder.Services.AddScoped<CreatePaymentUseCase>();
        builder.Services.AddScoped<CreateTransferUseCase>();
        builder.Services.AddScoped<DeletePaymentOrRelatedTransferIfAnyUseCase>();
        builder.Services.AddScoped<DeleteTransferUseCase>();
        builder.Services.AddScoped<GetAllAccountsUseCase>();
        builder.Services.AddScoped<GetAllConfigsUseCase>();
        builder.Services.AddScoped<GetBanksUseCase>();
        builder.Services.AddScoped<GetMonthUseCase>();
        builder.Services.AddScoped<LoginUseCase>();
        builder.Services.AddScoped<RecurrenceService>();
        builder.Services.AddScoped<RefreshTokenUseCase>();
        builder.Services.AddScoped<UpdateConfigUseCase>();
        builder.Services.AddScoped<EditPaymentUseCase>();
        builder.Services.AddScoped<EditTransferUseCase>();
        builder.Services.AddScoped<UserService>();
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
        app.UseMiddleware(typeof(ErrorHandlingMiddleware));
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}