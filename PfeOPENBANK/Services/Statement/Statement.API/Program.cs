using Account.Grpc.Protos;
using Branch.GRPC.Protos;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Statement.API.Services;
using Statement.API.Services.Grpc;
using Statement.API.Services.Interfaces;
using Helper.Extensions;
using Helper.Middlewares;
using Helper.Utils.Interfaces;
using Helper.Utils;
using Helper.Applications.Performances.Performances;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ApplyGdpr();
// Add services to the container.
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<BranchService>();

builder.Services.AddTransient<TokenManagerMiddleware>();
builder.Services.AddTransient<ITokenManager, TokenManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedRedisCache(r => r.Configuration = builder.Configuration["redis:connectionString"]);

builder.Services.AddHttpClient<IStatementService, StatementService>(c =>
                c.BaseAddress = new Uri(builder.Configuration["OracleSettings:OrdsDatabaseUrl"]));

builder.Services.AddGrpcClient<BranchProtoService.BranchProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:BranchUrl"]);
});

builder.Services.AddGrpcClient<AccountProtoService.AccountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:AccountUrl"]);
});

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
                     IndexFormat = "Statement.api-logs-" +
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