using System;
using System.Text.Json;
using System.Threading;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;

        public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler)
        {
            _config = config.Value;
            _eventHandler = eventHandler;
        }

        public void Consume(string topic, CancellationToken cancellationToken)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                        .SetKeyDeserializer(Deserializers.Utf8)
                        .SetValueDeserializer(Deserializers.Utf8)
                        .Build();

            consumer.Subscribe(topic);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult?.Message == null) continue;

                        var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };

                        var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);

                        if (@event == null) continue;

                        var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });

                        if (handlerMethod == null)
                        {
                            throw new ArgumentException(nameof(handlerMethod), "couldn't find event handler method!");
                        }

                        handlerMethod.Invoke(_eventHandler, new object[] { @event });
                        consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException ex)
                    {
                        var reason = ex.Error.Reason ?? string.Empty;
                        if (reason.IndexOf("unknown topic", StringComparison.OrdinalIgnoreCase) >= 0 || reason.IndexOf("unknown topic or partition", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(1));
                            continue;
                        }

                        throw;
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}