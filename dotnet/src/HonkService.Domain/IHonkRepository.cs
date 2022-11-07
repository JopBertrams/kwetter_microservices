using HonkService.Domain.Entities;

namespace HonkService.Domain
{
    public interface IHonkRepository
    {
        Task<HonkEntity> GetHonkById(Guid honkId);

        Task<HonkEntity> AddAsync(HonkEntity honkEntity);

        Task<Guid> DeleteAsync(Guid honkId);
    }
}
