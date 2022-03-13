using MassTransit;
using payment;

namespace Consumer.Consumer;

public class KafkaPaymentConsumer :
    IConsumer<Payment>
{
    public Task Consume(ConsumeContext<Payment> context)
    {
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine($"$ Message consumed: {context.MessageId}");
        Console.WriteLine($"Payment name: {context.Message.Name}");
        Console.WriteLine($"Payment price: {context.Message.Price}");
        Console.WriteLine("------------------------------------------------------");

        return Task.CompletedTask;
    }
}
