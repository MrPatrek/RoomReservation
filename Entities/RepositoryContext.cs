using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Room>? Rooms { get; set; }
        public DbSet<Reservation>? Reservations { get; set; }
    }
}
