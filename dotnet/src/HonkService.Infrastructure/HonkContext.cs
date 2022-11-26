using HonkService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HonkService.Infrastructure
{
    public class HonkContext : DbContext
    {
        public HonkContext(DbContextOptions<HonkContext> options) : base(options) { }

        public DbSet<HonkEntity> Honks { get; set; }
    }
}
