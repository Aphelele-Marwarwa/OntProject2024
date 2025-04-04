using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ont3010_Project_YA2024.Models.admin;
using Ont3010_Project_YA2024.Models.CustomerLiaison;
using Ont3010_Project_YA2024.Models.InventoryLiaison;
using Ont3010_Project_YA2024.Models;
using Ont3010_Project_YA2024.Data.Notifications;
using Ont3010_Project_YA2024.Models.CustomerReport;






namespace Ont3010_Project_YA2024.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(TimeSpan.FromSeconds(60));
        }


        public DbSet<Employee> Employees { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Fridge> Fridges { get; set; }
        public DbSet<FridgeAllocation> FridgeAllocations { get; set; }
        public DbSet<ScrappedFridge> ScrappedFridges { get; set; }
        public DbSet<ProcessAllocation> ProcessAllocations { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmployeeNotificationStatus> EmployeeNotificationStatuses { get; set; }
       
        public DbSet<NewFridgeRequest> NewFridgeRequests { get; set; }
   
       


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure composite key for Fridge
            modelBuilder.Entity<Fridge>()
                .HasKey(f => new { f.FridgeId, f.SerialNumber });

            modelBuilder.Entity<Fridge>()
                .Property(f => f.FridgeId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Fridge>()
                .Property(f => f.SerialNumber)
                .IsRequired();

            // Configure FridgeAllocation entity
            modelBuilder.Entity<FridgeAllocation>()
                .HasKey(fa => fa.FridgeAllocationId);

            modelBuilder.Entity<FridgeAllocation>()
                .HasOne(fa => fa.Fridge)
                .WithMany(f => f.FridgeAllocations)
                .HasForeignKey(fa => new { fa.FridgeId, fa.SerialNumber })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeAllocation>()
                .HasOne(fa => fa.Customer)
                .WithMany(c => c.FridgeAllocations)
                .HasForeignKey(fa => fa.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FridgeAllocation>()
                .HasOne(fa => fa.Employee)
                .WithMany(e => e.FridgeAllocations)
                .HasForeignKey(fa => fa.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ProcessAllocation entity
            modelBuilder.Entity<ProcessAllocation>()
                .HasKey(pa => pa.ProcessAllocationId);

            modelBuilder.Entity<ProcessAllocation>()
                .HasOne(pa => pa.FridgeAllocation)
                .WithMany(fa => fa.ProcessAllocations)
                .HasForeignKey(pa => pa.FridgeAllocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProcessAllocation>()
                .HasOne(pa => pa.Fridge)
                .WithMany(f => f.ProcessAllocations)
                .HasForeignKey(pa => new { pa.FridgeId, pa.SerialNumber })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProcessAllocation>()
                .HasOne(pa => pa.Customer)
                .WithMany(c => c.ProcessAllocations)
                .HasForeignKey(pa => pa.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PurchaseRequest entity
            modelBuilder.Entity<PurchaseRequest>()
                .HasKey(pr => pr.PurchaseRequestId);

            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.Fridge)
                .WithMany(f => f.PurchaseRequests)
                .HasForeignKey(pr => new { pr.FridgeId, pr.SerialNumber })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.Employee)
                .WithMany(e => e.PurchaseRequests)
                .HasForeignKey(pr => pr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ScrappedFridge entity
            modelBuilder.Entity<ScrappedFridge>()
                .HasKey(sf => sf.ScrappedFridgeId);

            modelBuilder.Entity<ScrappedFridge>()
                .HasOne(sf => sf.Fridge)
                .WithMany(f => f.ScrappedFridges)
                .HasForeignKey(sf => new { sf.FridgeId, sf.FridgeSerialNumber })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScrappedFridge>()
                .HasOne(sf => sf.Employee)
                .WithMany(e => e.ScrappedFridges)
                .HasForeignKey(sf => sf.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
            .HasMany(c => c.FridgeAllocations)
            .WithOne(f => f.Customer)
            .HasForeignKey(f => f.CustomerId);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.ProcessAllocations)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId);
            base.OnModelCreating(modelBuilder);

           

          }




    }
}