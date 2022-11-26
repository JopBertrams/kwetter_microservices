using HonkService.Application.Common;
using HonkService.Domain.Entities;
using HonkService.Domain;
using Mapster;
using MediatR;

namespace HonkService.Application.Commands
{
    public class DeleteHonkCommandHandler : IRequestHandler<DeleteHonkCommand, DeletedHonkResult>
    {
        private readonly IHonkRepository _honkRepository;

        public DeleteHonkCommandHandler(IHonkRepository honkRepository)
        {
            _honkRepository = honkRepository ?? throw new ArgumentNullException(nameof(honkRepository));
        }

        public async Task<DeletedHonkResult> Handle(DeleteHonkCommand command, CancellationToken cancellationToken)
        {
            await _honkRepository.DeleteAsync(command.honkId);

            return new DeletedHonkResult(command.honkId);
        }
    }
}
