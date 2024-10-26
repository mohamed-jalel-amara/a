using Account.Grpc.Protos;
using EventBus.Message.Common;
using MassTransit;
using MediatR;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Transaction.API.EventBusConsumer;
using Transaction.API.Services;
using Transaction.API.Services.Grpc;
using Transaction.API.Services.Interfaces;
using Helper.Extensions;
using Helper.Middlewares;
using Bank.grpc.Protos;
using View.grpc.Protos;
using User.grpc.Protos;
using Transaction.API.Utils.Models;
using Helper.Utils.Interfaces;
using Helper.Utils;
using Helper.Applications.Performances.Performances;

var builder = WebApplication.CreateBuilder(args);
// bindings
builder.Services.ApplyGdpr();
//TransactionRequestType.Types = builder.Configuration.GetValue<List<string>>("Transaction:AllowedTypes") ?? new List<string>();


// Add services to the container.
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<BankService>();
builder.Services.AddScoped<ViewService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddTransient<TokenManagerMiddleware>();
builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedRedisCache(r => r.Configuration = builder.Configuration["redis:connectionString"]);

builder.Services.AddHttpClient<ITransactionService, TransactionService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["OracleSettings:OrdsDatabaseUrl"]));

builder.Services.AddGrpcClient<AccountProtoService.AccountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:AccountUrl"]);
});

builder.Services.AddGrpcClient<BankProtoService.BankProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:BankUrl"]);
});

builder.Services.AddGrpcClient<ViewProtoService.ViewProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:AccountAccessUrl"]);
});

builder.Services.AddGrpcClient<UserProtoService.UserProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:UserUrl"]);
});

//RabbitMQ & Masstransit configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<AccountConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.AccountQueue, c =>
        {
            c.ConfigureConsumer<AccountConsumer>(ctx);
        });
    });
});
builder.Services.AddMassTransitHostedService();

//MediatR Configuration
builder.Services.AddMediatR(typeof(Program));
//AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers(options => options.Filters.Add<LogRequestTimeFilterAttribute>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuration of Serilog (ELK)
builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext()
                 .Enrich.WithMachineName()
                 .WriteTo.Console()
                 .WriteTo.Debug()
                 .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticConfiguration:Uri"]))
                 {
                     AutoRegisterTemplate = true,
                     IndexFormat = "Transaction.api-logs-" +
                     $"{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                     NumberOfReplicas = 1,
                     NumberOfShards = 2,
                 })
                 .Enrich.WithProperty("Environnement", context.HostingEnvironment.EnvironmentName)
                 .ReadFrom.Configuration(context.Configuration);
});

#region Configuration of Authorization with JWT
builder.Services.AddJwtAuthentication(builder.Configuration);
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<TokenManagerMiddleware>();

app.UseCookiePolicy();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
