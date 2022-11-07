using HonkService.Domain;
using HonkService.Domain.Entities;

namespace HonkService.Infrastructure.Repositories
{
    public class HonkRepository : IHonkRepository
    {
        private readonly HonkContext _honkContext;

        public HonkRepository(HonkContext honkContext)
        {
            _honkContext = honkContext ?? throw new ArgumentNullException(nameof(honkContext));
        }

        public async Task<HonkEntity> AddAsync(HonkEntity honkEntity)
        {
            _honkContext.Honks.Add(honkEntity);
            await _honkContext.SaveChangesAsync();
            return honkEntity;
        }

        public async Task<Guid> DeleteAsync(Guid honkId)
        {
            var honk = await _honkContext.Honks.FindAsync(honkId);
            if (honk == null)
                throw new KeyNotFoundException();

            _honkContext.Honks.Remove(honk);
            await _honkContext.SaveChangesAsync();
            return honkId;
        }

        public async Task<HonkEntity> GetHonkById(Guid honkId)
        {
            var honk = await _honkContext.Honks.FindAsync(honkId);
            if (honk == null)
                throw new KeyNotFoundException();

            return honk;
        }
    }
}
