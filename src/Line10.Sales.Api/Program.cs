using Line10.Sales.Api.BackgroundServices;
using Line10.Sales.Api.Endpoints;
using Line10.Sales.Domain.Persistence;
using Line10.Sales.Infrastructure;
using Line10.Sales.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Api endpoints
app.AddCustomerEndpoints();
app.AddProductEndpoints();

app.Run();

public partial class Program;