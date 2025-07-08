using FashionShopSystem.API.Middlewares;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories.UserRepo;
using FashionShopSystem.Infrastructure.Repositories.OrderRepo;
using FashionShopSystem.Service.Services.UserService;
using FashionShopSystem.Service.Services.OrderService;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Service;

namespace FashionShopSystem.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			//odata support
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
					options.JsonSerializerOptions.PropertyNamingPolicy = null;
				});

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

			//Add cors
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontend", policy =>
				{
					policy.AllowAnyOrigin()
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});

			// Add Database configuration

			// Add Fluent Validation
			builder.Services.AddFluentValidationAutoValidation();
			builder.Services.AddDbContext<FashionShopContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
			// Add UserService and UserRepository
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
			builder.Services.AddScoped<IProductRepo, ProductRepo>();
			builder.Services.AddScoped<IFavouriteRepo, FavouriteRepo>();
			builder.Services.AddScoped<IFavouriteService, FavouriteService>();
            builder.Services.AddScoped<IProductService>(provider =>
            {
                var productRepo = provider.GetRequiredService<IProductRepo>();
                var env = provider.GetRequiredService<IWebHostEnvironment>();

                return new ProductService(productRepo, env.WebRootPath);
            });
            // Add OrderService and OrderRepository
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

			// Add OrderService and OrderRepository
			builder.Services.AddScoped<IOrderRepository, OrderRepository>();
			builder.Services.AddScoped<IOrderService, OrderService>();

			// Add JWT authentication
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
					IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
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
