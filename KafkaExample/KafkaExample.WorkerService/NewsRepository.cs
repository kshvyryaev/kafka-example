using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KafkaExample.WorkerService
{
	public class NewsRepository : INewsRepository
	{
		private readonly IMongoDatabase _database;

		public NewsRepository(IMongoDatabase database)
		{
			_database = database;
		}

		protected IMongoCollection<News> Collection => _database.GetCollection<News>(nameof(News));

		public async Task ReplaceNewsAsync(News news)
		{
			news.Id = ObjectId.GenerateNewId();

			await Collection.ReplaceOneAsync(e => e.Id.Equals(news.Id), news, new ReplaceOptions
			{
				IsUpsert = true
			});
		}
	}
}
