using FCG.MS.Payments.Application.DTOs;
using FCG.MS.Payments.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FCG.MS.Payments.Infrastructure.Messaging;

public class PaymentConsumerService : BackgroundService
{
    private readonly ConnectionFactory _factory;
    private readonly IServiceProvider _serviceProvider;

    public PaymentConsumerService(IConfiguration config, IServiceProvider serviceProvider)
    {
        _factory = new ConnectionFactory
        {
            Uri = new Uri(config["RabbitMQ:Connection"])
        };
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var connection = await _factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "payments_queue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var createCheckoutRequest = JsonSerializer.Deserialize<CreateCheckoutRequest>(message);

                using var scope = _serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                if (createCheckoutRequest != null)
                    await paymentService.CreateCheckoutAsync(createCheckoutRequest);

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync(
            queue: "payments_queue",
            autoAck: false,
            consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

    }
}