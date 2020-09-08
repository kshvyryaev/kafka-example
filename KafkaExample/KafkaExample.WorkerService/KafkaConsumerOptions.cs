namespace KafkaExample.WorkerService
{
	public class KafkaConsumerOptions
	{
		public string GroupId { get; set; }

		public string BootstrapServers { get; set; }
	}
}
