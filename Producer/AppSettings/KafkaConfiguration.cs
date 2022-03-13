namespace Producer.AppSettings;

public class KafkaConfiguration
{
    public string PaymentEventTopic { get; set; } = null!;
    public string PaymentListener { get; set; } = null!;
    public string SchemaRegistryUrl { get; set; } = null!;
    public string KafkaBroker { get; set; } = null!;
}
