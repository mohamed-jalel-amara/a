using Microsoft.AspNetCore.Identity;
using Identity.API.Applications.Data;
using Identity.API.Applications.Security;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Identity.API.Extensions;
using Identity.API.Services.Interfaces;
using Identity.API.Services;
using EventBus.Message.Common;
using MassTransit;
using Identity.API.Applications.Middlewares;
using Identity.API.EventBusConsumer;
using Microsoft.EntityFrameworkCore;
using Identity.API.Applications.Models.Entities;
using Identity.API.Services.Grpc;
using Bank.grpc.Protos;
using Identity.API.Utils.Interfaces;
using Identity.API.Utils;
using Identity.API.Applications.Performances.Performances;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;

    options.MinimumSameSitePolicy = SameSiteMode.None;
    //options.ConsentCookieValue = "true";
});

// Add services to the container.
builder.Services.AddTransient<TokenManagerMiddleware>();
builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedRedisCache(r => r.Configuration = builder.Configuration["redis:connectionString"]  );

builder.Services.AddControllers(options => options.Filters.Add<LogRequestTimeFilterAttribute>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEntitlementService, EntitlementService>();
builder.Services.AddScoped<BankService>();

builder.Services.AddGrpcClient<BankProtoService.BankProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:BankUrl"]);
});

//Configuration du context avec la chaine de connection
builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnectionString"));
});

//configuration de Identity User
builder.Services.AddDefaultIdentity<UserModel>(config =>
{
    config.SignIn.RequireConfirmedEmail = false;
    config.SignIn.RequireConfirmedAccount = false;
    config.Password.RequireNonAlphanumeric = false;

}).AddRoles<Entitlement>()
  .AddRoleManager<RoleManager<Entitlement>>()
  .AddUserManager<UserManager<UserModel>>()
  .AddDefaultTokenProviders()
  .AddSignInManager()
  .AddEntityFrameworkStores<IdentityContext>();


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
                     IndexFormat = "Auth.api-logs-" +
                     $"{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                     NumberOfReplicas = 1,
                     NumberOfShards = 2,
                 })
                 .Enrich.WithProperty("Environnement", context.HostingEnvironment.EnvironmentName)
                 .ReadFrom.Configuration(context.Configuration);
});

//RabbitMQ & Masstransit configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<CreationAccountConsumer>();
    config.AddConsumer<GrantEntitlementConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.CreationAccountQueue, c =>
        {
            c.ConfigureConsumer<CreationAccountConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstants.GrantEntitlementQueue, c =>
        {
            c.ConfigureConsumer<GrantEntitlementConsumer>(ctx);
        });
    });
});
builder.Services.AddMassTransitHostedService();

//AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(Program));

//Configuration de la partie JWT Tokens
builder.Services.AddCustomAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
//migration automatique vers la base de donnees
app.MigrateDatabase<IdentityContext>((context, services) => {});
//Creation super admin
await app.CreateSuperAdmin<IdentityContext>((context, services) => { });

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
