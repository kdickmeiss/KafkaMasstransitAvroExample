using MassTransit.KafkaIntegration;
using Microsoft.AspNetCore.Mvc;
using payment;
using Producer.Dto;

namespace Producer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducePaymentMessageController : ControllerBase
{
    private readonly ITopicProducer<string, Payment> _producer;

    public ProducePaymentMessageController(ITopicProducer<string, Payment> producer)
    {
        _producer = producer;
    }

    [HttpPost]
    public async Task<ActionResult> ProducePaymentMessageAsync([FromBody] IEnumerable<PaymentDto> paymentMessages)
    {
        var random = new Random();

        async Task ProduceMessage(Guid key, Payment value) =>
            await _producer.Produce(key.ToString(), value);

        async Task Wait(int min, int max) =>
            await Task.Delay(random.Next(min, max));

        foreach (var paymentMessage in paymentMessages)
        {
            var id = Guid.NewGuid();
            await ProduceMessage(id, new Payment { Name = paymentMessage.Name, Price = paymentMessage.Price});
            await Wait(1500, 2500);
        }
        return Ok("All messages has been send");
    }
}
