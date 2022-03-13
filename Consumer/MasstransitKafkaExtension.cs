using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Consumer.AppSettings;
using Consumer.Consumer;
using MassTransit;
using payment;

namespace Consumer;

public static class MasstransitKafkaExtension
{
    public static void AddMasstransitKafkaServiceExtension(this IServiceCollection services,
        KafkaConfiguration kafkaConfiguration)
    {
        var schemaRegistryConfig = new SchemaRegistryConfig {Url = kafkaConfiguration.SchemaRegistryUrl};
        var schemaRegistryClient = new CachedSchemaRegistryClient(schemaRegistryConfig);
        services.AddSingleton<ISchemaRegistryClient>(schemaRegistryClient);

        services.AddMassTransit(busConfig =>
        {
            busConfig.UsingInMemory((context, config) => config.ConfigureEndpoints(context));

            busConfig.AddRider(riderConfig =>
            {
                riderConfig.AddConsumersFromNamespaceContaining<KafkaPaymentConsumer>();
                riderConfig.UsingKafka((context, k) =>
                {
                    k.Host(kafkaConfiguration.KafkaBroker);
                    // TODO: use groupId from config
                    k.TopicEndpoint<string, Payment>(kafkaConfiguration.PaymentEventTopic, "payment-consumer", topicConfig =>
                    {
                        topicConfig.SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistryClient).AsSyncOverAsync());
                        topicConfig.SetValueDeserializer(new AvroDeserializer<Payment>(schemaRegistryClient).AsSyncOverAsync());
                        topicConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                        topicConfig.ConfigureConsumer<KafkaPaymentConsumer>(context);

                        topicConfig.CreateIfMissing(m =>
                        {
                            m.NumPartitions = 2;
                        });
                    });
                });
            });
        });
        services.AddMassTransitHostedService();
    }
}
