using BookingServiceProvider.Contexts;
using BookingServiceProvider.Factories;
using BookingServiceProvider.Interfaces;
using BookingServiceProvider.Repositories;
using BookingServiceProvider.Services;
using InvoiceServiceClient = InvoiceServiceProvider.InvoiceServiceContract.InvoiceServiceContractClient;
using Microsoft.EntityFrameworkCore;
using UserProfileServiceClient = UserProfileServiceProvider.UserProfileService.UserProfileServiceClient;
using EventServiceClient = EventServiceProvider.EventContract.EventContractClient;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddSingleton<BookingReplyFactory>();
builder.Services.AddSingleton<InvoiceRequestFactory>();

builder.Services.AddLogging();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AzureDbConnection"))
);

builder.Services.AddGrpcClient<UserProfileServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Grpc:UserServiceProvider"]!);
});

builder.Services.AddGrpcClient<InvoiceServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Grpc:InvoiceServiceProvider"]!);
});

builder.Services.AddGrpcClient<EventServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Grpc:EventServiceProvider"]!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<BookingService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
