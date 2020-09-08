using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Confluent.Kafka;
using KafkaExample.Api.Options;
using KafkaExample.Api.Models.News;
using KafkaExample.Contracts.Events;

namespace KafkaExample.Api.Controllers
{
	[ApiController]
	[Route("news")]
	public class NewsController : ControllerBase
	{
		private const string Topic = "kafka-example-topic";

		private readonly ProducerBuilder<Null, string> _producerBuilder;

		public NewsController(IOptions<KafkaProducerOptions> kafkaProducerOptions)
		{
			var options = kafkaProducerOptions.Value;

			var config = new ProducerConfig
			{
				BootstrapServers = options.BootstrapServers
			};

			_producerBuilder = new ProducerBuilder<Null, string>(config);
		}

		[HttpPost]
		public async Task<IActionResult> CreateAsync([FromBody] CreateNewsCommand command)
		{
			if (command == null)
			{
				return BadRequest();
			}

			var @event = new CreateNewsEvent
			{
				Name = command.Name,
				Description = command.Description
			};

			var value = JsonSerializer.Serialize(@event);

			var message = new Message<Null, string>
			{
				Value = value
			};

			using var producer = _producerBuilder.Build();
			
			await producer.ProduceAsync(Topic, message);

			return Ok();
		}
	}
}
