using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using TimelineService.Domain.Entities;
using System.Text;
using TimelineService.Application.Commands;
using TimelineService.Domain;

namespace TimelineService.Application.Services
{
    public class HonkDataCollector : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ITimelineRepositoy _timelineRepository;

        public HonkDataCollector(ITimelineRepositoy timelineRepositoy)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://gzzpbcle:b4SqtWxK0tLpbeFa7roTWnkT4bfxmQBm@sparrow.rmq.cloudamqp.com/gzzpbcle")
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _timelineRepository = timelineRepositoy;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var queueName = "honk_queue";
            bool durable = true;
            bool exclusive = false;
            bool autoDelete = false;

            _channel.QueueDeclare(queueName, durable, exclusive, autoDelete, null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, deliveryEventArgs) =>
            {
                var body = deliveryEventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    var honk = JsonConvert.DeserializeObject<HonkEntity>(message);
                    Console.WriteLine(honk);
                    var command = new CreateTimelineCommand(honk);
                    var createTimelineHandler = new CreateTimelineCommandHandler(_timelineRepository);
                    await createTimelineHandler.Handle(command, cancellationToken);
                }
                catch{}
                
                _channel.BasicAck(deliveryEventArgs.DeliveryTag, false);
            };

            _channel.BasicConsume(consumer, queueName);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
