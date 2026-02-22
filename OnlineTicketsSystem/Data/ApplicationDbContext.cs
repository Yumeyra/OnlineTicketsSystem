using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Models.Common;

namespace OnlineTicketsSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
          
            builder.Entity<Event>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Ticket>().HasQueryFilter(t => !t.IsDeleted);
            builder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<City>().HasQueryFilter(c => !c.IsDeleted);
            builder.Entity<Favorite>().HasQueryFilter(f => !f.IsDeleted);
            builder.Entity<Review>().HasQueryFilter(r => !r.IsDeleted);
            builder.Entity<Order>().HasQueryFilter(o => !o.IsDeleted);
            builder.Entity<OrderItem>().HasQueryFilter(oi => !oi.IsDeleted);


            builder.Entity<Ticket>().Property(t => t.UnitPrice).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            builder.Entity<Favorite>()
        .HasIndex(f => new { f.UserId, f.EventId })
        .IsUnique();
        }

            public override int SaveChanges()
        {
            ApplySoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplySoftDelete()
        {
            var deletedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDeletable);

            foreach (var entry in deletedEntries)
            {
                entry.State = EntityState.Modified;
                var entity = (ISoftDeletable)entry.Entity;
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
            }
        }
    
        

    }

}

