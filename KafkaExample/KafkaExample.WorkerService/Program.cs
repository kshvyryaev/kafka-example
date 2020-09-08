using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace KafkaExample.WorkerService
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					var configuration = hostContext.Configuration;
					services.Configure<KafkaConsumerOptions>(configuration.GetSection("KafkaConsumer"));

					var mongoOptions = new MongoOptions();
					configuration.GetSection("Mongo").Bind(mongoOptions);
					var mongoClient = new MongoClient(mongoOptions.ConnectionString);
					var mongoDatabase = mongoClient.GetDatabase(mongoOptions.Database);
					services.AddSingleton(mongoDatabase);

					services.AddSingleton<INewsRepository, NewsRepository>();

					services.AddHostedService<Worker>();
				});
	}
}
