using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KafkaExample.WorkerService
{
	public class News
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}
