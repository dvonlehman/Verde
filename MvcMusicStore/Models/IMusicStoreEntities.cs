using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace MvcMusicStore.Models
{
    public interface IMusicStoreEntities : IDisposable
    {
        DbSet<Album> Albums { get; set; }
        DbSet<Artist> Artists { get; set; }
        DbSet<Cart> Carts { get; set; }
        DbSet<Genre> Genres { get; set; }
        DbSet<OrderDetail> OrderDetails { get; set; }
        DbSet<Order> Orders { get; set; }

        DbEntityEntry Entry(object entity);

        int SaveChanges();
    }
}
