using Microsoft.EntityFrameworkCore;
using MongoDB;
using MongoDB.Bson;
using PromotionCodes.Db.Entities;

namespace PromotionCodes.Db;

public class PromoCodeDbContext : DbContext
{
    public PromoCodeDbContext (DbContextOptions<PromoCodeDbContext> options) : base (options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PromoCodeEntry>().ToJson();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

}
