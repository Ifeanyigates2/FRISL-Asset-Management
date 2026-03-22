using System.Threading.Channels;

namespace FrislEams.Web.Services;

public record IntegrationJob(string EventType, string SourceSystem, string Payload);

public interface IIntegrationQueue
{
    ValueTask QueueAsync(IntegrationJob job, CancellationToken cancellationToken = default);
    ValueTask<IntegrationJob> DequeueAsync(CancellationToken cancellationToken);
}

public class IntegrationQueue : IIntegrationQueue
{
    private readonly Channel<IntegrationJob> _channel = Channel.CreateUnbounded<IntegrationJob>();

    public ValueTask QueueAsync(IntegrationJob job, CancellationToken cancellationToken = default)
    {
        return _channel.Writer.WriteAsync(job, cancellationToken);
    }

    public ValueTask<IntegrationJob> DequeueAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}
