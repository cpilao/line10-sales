using System.Text.Json.Serialization;
using Line10.Sales.Api.BackgroundServices;
using Line10.Sales.Api.Cache;
using Line10.Sales.Api.Endpoints;
using Line10.Sales.Api.Swagger;
using Line10.Sales.Domain.Persistence;
using Line10.Sales.Infrastructure;
using Line10.Sales.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<EnumSchemaFilter>();
});

// json settings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Application
builder.Services.AddMediatR(o =>
{
    o.RegisterServicesFromAssembly(typeof(Line10.Sales.Application.IAnchor).Assembly);
});

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

var app = builder.Build();

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