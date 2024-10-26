
namespace Branch.API.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvailability = retry.Value;

            using (var scope = host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;
                var config = service.GetRequiredService<IConfiguration>();
                var logger = service.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrated Oracle database!");
                   

                    logger.LogInformation("Migrated Oracle database!");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);

                    if (retryForAvailability < 5)
                    {
                        retryForAvailability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvailability);
                    }
                }
            }
            return host;
        }
    }
}
