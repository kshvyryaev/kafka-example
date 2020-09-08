using System.Threading.Tasks;

namespace KafkaExample.WorkerService
{
	public interface INewsRepository
	{
		Task ReplaceNewsAsync(News news);
	}
}
