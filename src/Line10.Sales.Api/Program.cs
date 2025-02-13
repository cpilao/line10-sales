using System.Text.Json.Serialization;
using FluentValidation;
using Line10.Sales.Api.BackgroundServices;
using Line10.Sales.Api.Cache;
using Line10.Sales.Api.Endpoints;
using Line10.Sales.Api.Security;
using Line10.Sales.Api.Swagger;
using Line10.Sales.Application;
using Line10.Sales.Application.Behaviors;
using Line10.Sales.Domain.Persistence;
using Line10.Sales.Infrastructure;
using Line10.Sales.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// log services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
    
    // Add JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
});

// json settings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Application
builder.Services.AddMediatR(o =>
{
    o.RegisterServicesFromAssembly(typeof(IAnchor).Assembly);
    o.AddOpenBehavior(typeof(LoggingBehavior<,>));
    o.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
// Application validators
builder.Services.AddValidatorsFromAssemblyContaining<IAnchor>();

// Infrastructure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<MigrationService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add output caching services
builder.Services.AddOutputCache(o =>
{
    o.AddPolicy("ByIdCachePolicy", policy => policy
        .AddPolicy<ByIdCachePolicy>()
        .Expire(TimeSpan.FromMinutes(10)));
    
    o.AddPolicy("ByRequestPathCachePolicy", policy => policy
        .AddPolicy<ByRequestPathCachePolicy>()
        .Expire(TimeSpan.FromMinutes(10)));
});

// Security
var tokenOptions = new TokenValidationOptions();
builder.Configuration.GetSection("TokenValidation").Bind(tokenOptions);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters();
        options.Bind(tokenOptions);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicies();
});

var app = builder.Build();

// Security middleware
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add the output caching middleware
app.UseOutputCache();

// Api endpoints
app.AddCustomerEndpoints();
app.AddProductEndpoints();
app.AddOrderEndpoints();

app.Run();

public partial class Program;