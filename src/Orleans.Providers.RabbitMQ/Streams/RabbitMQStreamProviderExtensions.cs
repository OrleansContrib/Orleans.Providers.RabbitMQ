using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans.Providers.RabbitMQ.Streams;

namespace Orleans.Runtime.Configuration
{
    public static class RabbitMQStreamProviderExtensions
    {
        public static void AddRabbitMQStreamProvider(
            this ClusterConfiguration config,
            string providerName)
        {
            config.Globals.RegisterStreamProvider<RabbitMQStreamProvider>(providerName);
        }

        public static void AddRabbitMQStreamProvider(
            this ClientConfiguration config,
            string providerName)
        {
            config.RegisterStreamProvider<RabbitMQStreamProvider>(providerName);
        }

        public static ISiloHostBuilder ConfigureRabbitMQStreamProvider(this ISiloHostBuilder builder, RabbitMQStreamProviderOptions options)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRabbitMQMapper>(provider => new RabbitMQDefaultMapper(provider.GetRequiredService<ILoggerFactory>()));
                services.AddSingleton(options);
            });
        }

        public static IClientBuilder ConfigureRabbitMQStreamProvider(this IClientBuilder builder, RabbitMQStreamProviderOptions options)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IRabbitMQMapper>(provider => new RabbitMQDefaultMapper(provider.GetRequiredService<ILoggerFactory>()));
                services.AddSingleton(options);
            });
        }
    }
}
