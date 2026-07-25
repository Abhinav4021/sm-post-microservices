using System.Threading;

namespace CQRS.Core.Consumers
{
    public interface IEventConsumer
    {
        void Consume(string topic, CancellationToken cancellationToken);
    }
}