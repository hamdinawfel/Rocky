using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rocky.Models;
using System.Data;

namespace Rocky.Data
{
    public class RockyDbContext : IdentityDbContext
    {
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public RockyDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
