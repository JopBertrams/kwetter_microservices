using HonkService.Application.Common;
using HonkService.Domain.Entities;
using HonkService.Domain;
using Mapster;
using MediatR;

namespace HonkService.Application.Commands
{
    public class PostHonkCommandHandler : IRequestHandler<PostHonkCommand, HonkResult>
    {
        private readonly IHonkRepository _honkRepository;

        public PostHonkCommandHandler(IHonkRepository honkRepository)
        {
            _honkRepository = honkRepository ?? throw new ArgumentNullException(nameof(honkRepository));
        }

        public async Task<HonkResult> Handle(PostHonkCommand command, CancellationToken cancellationToken)
        {
            HonkEntity honkEntity = command.Adapt<HonkEntity>();
            honkEntity.CreatedAt = DateTime.UtcNow;

            await _honkRepository.AddAsync(honkEntity);

            return honkEntity.Adapt<HonkResult>();
        }
    }
}
