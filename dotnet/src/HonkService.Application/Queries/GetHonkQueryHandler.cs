using HonkService.Application.Commands;
using HonkService.Application.Common;
using HonkService.Domain.Entities;
using HonkService.Domain;
using MediatR;
using Mapster;

namespace HonkService.Application.Queries
{
    public class GetHonkQueryHandler : IRequestHandler<GetHonkQuery, HonkResult>
    {
        private readonly IHonkRepository _honkRepository;

        public GetHonkQueryHandler(IHonkRepository honkRepository)
        {
            _honkRepository = honkRepository ?? throw new ArgumentNullException(nameof(honkRepository));
        }

        public async Task<HonkResult> Handle(GetHonkQuery query, CancellationToken cancellationToken)
        {
            HonkEntity honkEntity = await _honkRepository.GetHonkById(query.honkId);

            return honkEntity.Adapt<HonkResult>();
        }
    }
}
