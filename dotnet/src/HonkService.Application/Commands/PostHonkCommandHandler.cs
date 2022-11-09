using HonkService.Application.Common;
using HonkService.Domain.Entities;
using HonkService.Domain;
using Mapster;
using MediatR;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace HonkService.Application.Commands
{
    public class PostHonkCommandHandler : IRequestHandler<PostHonkCommand, HonkResult>
    {
        private readonly IHonkRepository _honkRepository;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public PostHonkCommandHandler(IHonkRepository honkRepository)
        {
            _honkRepository = honkRepository ?? throw new ArgumentNullException(nameof(honkRepository));

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://gzzpbcle:b4SqtWxK0tLpbeFa7roTWnkT4bfxmQBm@sparrow.rmq.cloudamqp.com/gzzpbcle")
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("honk_queue", true, false, false, null);
        }

        public async Task<HonkResult> Handle(PostHonkCommand command, CancellationToken cancellationToken)
        {
            HonkEntity honkEntity = command.Adapt<HonkEntity>();
            honkEntity.CreatedAt = DateTime.UtcNow;

            await _honkRepository.AddAsync(honkEntity);

            //Publish to RabbitMQ
            var serializedString = JsonConvert.SerializeObject(honkEntity);
            var body = Encoding.UTF8.GetBytes(serializedString);
            _channel.BasicPublish("honk_exchange", "timeline", false, null, body);


            return honkEntity.Adapt<HonkResult>();
        }
    }
}