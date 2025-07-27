using FashionShopSystem.API.Middlewares;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Infrastructure.Repositories.OrderDetailRepo;
using FashionShopSystem.Infrastructure.Repositories.OrderRepo;
using FashionShopSystem.Infrastructure.Repositories.UserRepo;
using FashionShopSystem.Service;
using FashionShopSystem.Service.AutoMapper;
using FashionShopSystem.Service.Services;
using FashionShopSystem.Service.Services.AuthService;
using FashionShopSystem.Service.Services.OrderService;
using FashionShopSystem.Service.Services.UserService;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Net.payOS;
using System.Text.Json;

namespace FashionShopSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // OData support
            builder.Services.AddControllers()
                .AddOData(options =>
                    options.Select()
                           .Filter()
                           .Count()
                           .OrderBy()
                           .Expand()
                           .SetMaxTop(100))
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            // Swagger config
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 123456'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // PayOS config
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            PayOS payOS = new PayOS(
                configuration["PayOS:ClientId"] ?? throw new Exception("Cannot find PayOS Client ID"),
                configuration["PayOS:ApiKey"] ?? throw new Exception("Cannot find PayOS API Key"),
                configuration["PayOS:ChecksumKey"] ?? throw new Exception("Cannot find PayOS Checksum Key")
            );
            builder.Services.AddSingleton(payOS);
            builder.Services.AddHttpContextAccessor();

            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Database
            builder.Services.AddDbContext<FashionShopContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

            // FluentValidation
            builder.Services.AddFluentValidationAutoValidation();

            // Dependency Injection
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddScoped<IProductRepo, ProductRepo>();
            builder.Services.AddScoped<IProductService>(provider =>
            {
                var productRepo = provider.GetRequiredService<IProductRepo>();
                var env = provider.GetRequiredService<IWebHostEnvironment>();
                return new ProductService(productRepo, env.WebRootPath);
            });

            builder.Services.AddScoped<IFavouriteRepo, FavouriteRepo>();
            builder.Services.AddScoped<IFavouriteService, FavouriteService>();

            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();


            // JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            //Config automapper
            builder.Services.AddAutoMapper(typeof(OrderProfile));

            var app = builder.Build();

            // Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseGlobalExceptionHandler();
            app.UseHttpsRedirection();
            app.UseForwardedHeaders();
            app.UseCors("AllowFrontend");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}