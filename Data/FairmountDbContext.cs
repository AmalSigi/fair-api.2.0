using FairMount_api.Models.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FairMount_api.Data
{
    public class FairmountDbContext : DbContext
    {
        public FairmountDbContext(DbContextOptions<FairmountDbContext> options)
          : base(options)
        {
        }

        // All DbSets consolidated based on your latest input and project requirements
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PurchaseOrders> PurchaseOrders { get; set; }
        public DbSet<POItem> POItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Organizations> Organizations { get; set; }
        public DbSet<OrganizationAddresses> OrganizationAddresses { get; set; }
        public DbSet<PoStatus> POStatus { get; set; }
        public DbSet<Potype> POType { get; set; }
        public DbSet<POitemStatus> POItemStatus { get; set; }
        public DbSet<ProformaInvoice> ProformaInvoices { get; set; }
        public DbSet<CommercialInvoice> CommercialInvoices { get; set; }
        public DbSet<CommercialInvoiceItem> CommercialInvoiceItems { get; set; }
        public DbSet<PackingList> PackingList { get; set; }
        public DbSet<PackingListItem> PackingListItem { get; set; }
        public DbSet<Sli_Document>SliDocuments { get; set; }
        public DbSet<Sli_Item> SliItem { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Existing POItem configuration
            modelBuilder.Entity<POItem>()
                .Property(p => p.TotalPrice)
                .ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        }
    }
}