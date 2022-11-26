using HonkService.Application.Common;
using MediatR;

namespace HonkService.Application.Queries
{
    public record GetHonkQuery(Guid honkId) : IRequest<HonkResult>;
}
