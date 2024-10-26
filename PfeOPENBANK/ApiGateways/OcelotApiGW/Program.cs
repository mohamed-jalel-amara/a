using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Cache.CacheManager;
using Helper.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        config.AddJsonFile($"ocelot.Local.json", true, true);
    })

    .ConfigureLogging((hostingContext, loggingBuilder) =>
    {
        loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        loggingBuilder.AddConsole();
        loggingBuilder.AddDebug();
    });

//Authentication JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
 {
     options.AddPolicy("CorsPolicy",
         builder => builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader());
 });

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();

app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

await app.UseOcelot();

app.Run();
