using Microsoft.EntityFrameworkCore;
using EnvaTest.Entities;

namespace EnvaTest.Context
{
    public class EnvaContext : DbContext
    {
        public EnvaContext(DbContextOptions<EnvaContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
    }
}
