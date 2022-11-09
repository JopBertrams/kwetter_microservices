using HonkService.Application.Common;
using HonkService.Domain.Entities;
using HonkService.Domain;
using Mapster;
using MediatR;
using Newtonsoft.Json;

namespace HonkService.Application.Commands
{
    public class PostHonkCommandHandler : IRequestHandler<PostHonkCommand, HonkResult>
    {
        private readonly IHonkRepository _honkRepository;
        private readonly Plain.RabbitMQ.IPublisher _publisher;

        public PostHonkCommandHandler(IHonkRepository honkRepository, Plain.RabbitMQ.IPublisher publisher)
        {
            _honkRepository = honkRepository ?? throw new ArgumentNullException(nameof(honkRepository));
            _publisher = publisher;
        }

        public async Task<HonkResult> Handle(PostHonkCommand command, CancellationToken cancellationToken)
        {
            HonkEntity honkEntity = command.Adapt<HonkEntity>();
            honkEntity.CreatedAt = DateTime.UtcNow;

            await _honkRepository.AddAsync(honkEntity);

            //Publish to RabbitMQ
            _publisher.Publish(JsonConvert.SerializeObject(honkEntity), "timeline", null);

            return honkEntity.Adapt<HonkResult>();
        }
    }
}