using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using MassTransit;
using MassTransit.KafkaIntegration;
using MassTransit.Registration;
using payment;
using Producer.AppSettings;

namespace Producer;

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
                // Set up producers - events are produced by DemoProducer hosted service
                AddPaymentProducer(riderConfig, kafkaConfiguration, schemaRegistryClient);

                riderConfig.UsingKafka((context, k) => { k.Host(kafkaConfiguration.KafkaBroker); });
            });
        });
        services.AddMassTransitHostedService();
    }

    private static void AddPaymentProducer(IRiderRegistrationConfigurator riderConfig,
        KafkaConfiguration kafkaConfiguration, CachedSchemaRegistryClient schemaRegistryClient)
    {
        riderConfig.AddProducer<string, Payment>(kafkaConfiguration.PaymentEventTopic, (riderContext, producerConfig) =>
        {
            // Note that all child serializers share the same AvroSerializerConfig - separate producers could
            // be used for each logical set of message types (e.g. all messages produced to a certain topic)
            // to support varying configuration if needed.
            producerConfig.SetKeySerializer(new AvroSerializer<string>(schemaRegistryClient)
                .AsSyncOverAsync());
            producerConfig.SetValueSerializer(new AvroSerializer<Payment>(schemaRegistryClient).AsSyncOverAsync());
        });
    }
}
