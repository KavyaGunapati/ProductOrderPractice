using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DataAccess.Entities;
namespace DataAccess.Context
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public DbSet<Product> Products { get; set; }=null!;
        public DbSet<Category> Categories { get; set; }=null!;
        public DbSet<OrderItem> OrderItems { get; set; }=null!;
        public DbSet<Order> Orders { get; set; }=null!;
        public DbSet<AppUser> AppUsers { get; set; }=null!;
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {    
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Product>()
            .HasMany(p => p.OrderItems)
            .WithOne(oi => oi.Product)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Order>()
            .HasOne(o=>o.Payment)
            .WithOne(p=>p.Order)
            .HasForeignKey<Payment>(p=>p.OrderId);
        }
    }
}