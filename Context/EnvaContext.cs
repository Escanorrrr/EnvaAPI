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
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }
        public DbSet<Formul> Formulas { get; set; }
    }
}
