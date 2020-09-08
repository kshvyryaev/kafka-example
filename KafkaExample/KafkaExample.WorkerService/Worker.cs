using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaExample.Contracts.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KafkaExample.WorkerService
{
	public class Worker : BackgroundService
	{
		private const string Topic = "kafka-example-topic";

		private readonly INewsRepository _newsRepository;
		private readonly IConsumer<Ignore, string> _consumer;

		public Worker(
			INewsRepository newsRepository,
			IOptions<KafkaConsumerOptions> kafkaConsumerOptions)
		{
			_newsRepository = newsRepository;

			var options = kafkaConsumerOptions.Value;

			var config = new ConsumerConfig
			{
				GroupId = options.GroupId ?? Guid.NewGuid().ToString(),
				BootstrapServers = options.BootstrapServers,
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = true
			};

			var builder = new ConsumerBuilder<Ignore, string>(config);

			_consumer = builder.Build();
			_consumer.Subscribe(Topic);
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var result = _consumer.Consume(stoppingToken);

				if (!result.IsPartitionEOF)
				{
					var @event = JsonSerializer.Deserialize<CreateNewsEvent>(result.Message.Value);

					var news = new News
					{
						Name = @event.Name,
						Description = @event.Description
					};

					await _newsRepository.ReplaceNewsAsync(news);
				}
			}
		}

		public override void Dispose()
		{
			_consumer.Unsubscribe();
			_consumer.Dispose();
		}
	}
}
