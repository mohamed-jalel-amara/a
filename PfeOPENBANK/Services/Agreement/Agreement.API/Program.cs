using Agreement.API.Services;
using Agreement.API.Services.Interfaces;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Helper.Extensions;
using Helper.Middlewares;
using Helper.Utils.Interfaces;
using Helper.Utils;
using Helper.Applications.Performances.Performances;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ApplyGdpr();
// Add services to the container.
builder.Services.AddTransient<TokenManagerMiddleware>();
builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedRedisCache(r => r.Configuration = builder.Configuration["redis:connectionString"]);

builder.Services.AddScheduler(builder =>
{
    builder.Services.AddScoped<AgreementService>();
    builder.AddJob<AgreementJob>();

    // register a custom error processing for internal errors
    builder.AddUnobservedTaskExceptionHandler(sp =>
    {
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("CronJobs");

        return
            (sender, args) =>
            {
                logger?.LogError(args.Exception?.Message);
                args.SetObserved();
            };
    });
});

builder.Services.AddHttpClient<IAgreementService, AgreementService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["OracleSettings:OrdsDatabaseUrl"]));

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
                     IndexFormat = "Account.access.api-logs-" +
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