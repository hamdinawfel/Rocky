using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rocky_Models;

namespace Rocky_DataAccess.Data
{
    public class RockyDbContext : IdentityDbContext
    {
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<InquiryDetail> InquiryDetail { get; set; }
        public DbSet<InquiryHeader> InquiryHeader { get; set; }
        public DbSet<Order> Order { get; set; }
        public RockyDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
