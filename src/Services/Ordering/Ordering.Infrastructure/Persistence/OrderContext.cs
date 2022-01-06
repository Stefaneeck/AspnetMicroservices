using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContext : DbContext
    {
        //in order  to connect with EF core
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
            //we haven't got any specific options here, but it is required for EF core
        }

        //will convert our dbtable
        //our Order class inherits from our EntityBase class
        public DbSet<Order> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //look for all entries that inherit from our EntityBase class
            //when you have some of the common fields from the entities, you can override the SaveChanges method from EF core and set these common columns before saving the actual entity in db
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "swn";
                        break;
                    case EntityState.Modified:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "swn";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
