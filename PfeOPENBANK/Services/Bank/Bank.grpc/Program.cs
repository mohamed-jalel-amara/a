using Bank.grpc.Services;
using Bank.grpc.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHttpClient<IBankService, BankService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["OrdsSettings:DatabaseUrl"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<BankServiceProvider>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
