using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;
using Post.Common.Events;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private static bool _mongoSerializerRegistered;
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository(IOptions<MongoDbConfig> config)
        {
            EnsureMongoSerializationRegistered();

            var mongoClient = new MongoClient(config.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(config.Value.DatabaseName);

            _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
        }

        private static void EnsureMongoSerializationRegistered()
        {
            if (_mongoSerializerRegistered)
            {
                return;
            }

            BsonSerializer.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.Standard));

            if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEvent)))
            {
                BsonClassMap.RegisterClassMap<BaseEvent>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true);
                    cm.SetDiscriminator(nameof(BaseEvent));
                });
            }

            RegisterEventClassMap<CommentAddedEvent>();
            RegisterEventClassMap<CommentRemovedEvent>();
            RegisterEventClassMap<CommentUpdatedEvent>();
            RegisterEventClassMap<MessageUpdatedEvent>();
            RegisterEventClassMap<PostCreatedEvent>();
            RegisterEventClassMap<PostLikedEvent>();
            RegisterEventClassMap<PostRemovedEvent>();

            _mongoSerializerRegistered = true;
        }

        private static void RegisterEventClassMap<TEvent>() where TEvent : BaseEvent
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TEvent)))
            {
                BsonClassMap.RegisterClassMap<TEvent>(cm => cm.AutoMap());
            }
        }
        

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }

        public async Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId)
        {
           return await _eventStoreCollection.Find(e => e.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);
        }
    }
}