using EventBus.Message.Common;
using MassTransit;
using MediatR;
using Notification.API.EventBusConsumer;
using Notification.API.Models;
using Notification.API.Services;
using Notification.API.Services.Interfaces;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Helper.Extensions;
using Helper.Applications.Performances.Performances;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ApplyGdpr();

// Add services to the container.
var sectionSms = builder.Configuration.GetSection("SmsSettings");
var optionSms = sectionSms.Get<SmsSettings>();
sectionSms.Bind(optionSms);
builder.Services.Configure<SmsSettings>(sectionSms);

builder.Services.AddTransient<ISenderService, SenderService>();

builder.Services.AddMediatR(typeof(Program));
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
                     IndexFormat = "Notification.api-logs-" +
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
    config.AddConsumer<EmailNotificationConsumer>();
    config.AddConsumer<SmsNotificationConsumer>();
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        cfg.ReceiveEndpoint(EventBusConstants.NotificationEmailQueue, c =>
        {
            c.ConfigureConsumer<EmailNotificationConsumer>(ctx);
        });
        cfg.ReceiveEndpoint(EventBusConstants.NotificationSmsQueue, c =>
        {
            c.ConfigureConsumer<SmsNotificationConsumer>(ctx);
        });
    });
});
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy();

app.UseAuthorization();

app.MapControllers();

app.Run();
