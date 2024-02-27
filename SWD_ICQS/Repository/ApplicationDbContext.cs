﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SWD_ICQS.Entity;
using System.Security.Principal;

namespace SWD_ICQS.Repository
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<BlogImages> BlogImages { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<ConstructImages> ConstructImages { get; set; }
        public DbSet<ConstructProducts> ConstructProducts { get; set; }
        public DbSet<Constructs> Constructs { get; set; }
        public DbSet<Contractors> Contractors { get; set; }
        public DbSet<Contracts> Contracts { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Products> ProductProducts { get; set; }
        public DbSet<Requests> Requests { get; set; }
        public DbSet<RequestDetails> RequestDetails { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Accounts

            // Appointments
            modelBuilder.Entity<Appointments>().HasOne(a => a.Contractor).WithMany(c => c.Appointments).HasForeignKey(a => a.ContractorId);
            modelBuilder.Entity<Appointments>().HasOne(a => a.Customer).WithMany(c => c.Appointments).HasForeignKey(a => a.CustomerId).OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder.Entity<Appointments>().HasOne(a => a.Contract).WithOne(c => c.Appointment).HasForeignKey<Appointments>(a => a.ContractId).OnDelete(DeleteBehavior.ClientSetNull);

            // BlogImages
            modelBuilder.Entity<BlogImages>().HasOne(b => b.Blog).WithMany(b => b.BlogImages).HasForeignKey(b => b.BlogId);

            // Blogs
            modelBuilder.Entity<Blogs>().HasOne(b => b.Contractor).WithMany(c => c.Blogs).HasForeignKey(b => b.ContractorId);

            // Categories

            // ConstructImages
            modelBuilder.Entity<ConstructImages>().HasOne(c => c.Construct).WithMany(c => c.ConstructImages).HasForeignKey(c => c.ConstructId);

            // ConstructProducts
            modelBuilder.Entity<ConstructProducts>().HasOne(c => c.Construct).WithMany(c => c.ConstructProducts).HasForeignKey(c => c.ContractorId);
            modelBuilder.Entity<ConstructProducts>().HasOne(c => c.Product).WithMany(p => p.ConstructProducts).HasForeignKey(c => c.ProductId).OnDelete(DeleteBehavior.ClientSetNull);

            // Constructs
            modelBuilder.Entity<Constructs>().HasOne(c => c.Contractor).WithMany(c => c.Constructs).HasForeignKey(c => c.ContractorId);
            modelBuilder.Entity<Constructs>().HasOne(p => p.Category).WithMany(c => c.Constructs).HasForeignKey(p => p.CategoryId);

            // Contractors
            modelBuilder.Entity<Contractors>().HasOne(c => c.Account).WithOne(a => a.Contractor).HasForeignKey<Contractors>(c => c.AccountId);
            modelBuilder.Entity<Contractors>().HasOne(c => c.Subscription).WithMany(s => s.Contractors).HasForeignKey(c => c.SubcriptionId);

            // Contracts
            modelBuilder.Entity<Contracts>().HasOne(c => c.Request).WithOne(r => r.Contract).HasForeignKey<Contracts>(c => c.RequestId);

            // Customers
            modelBuilder.Entity<Customers>().HasOne(c => c.Account).WithOne(a => a.Customer).HasForeignKey<Customers>(c => c.AccountId);

            // Messages
            modelBuilder.Entity<Messages>().HasOne(m => m.Contractor).WithMany(c => c.Messages).HasForeignKey(m => m.ContractorId);
            modelBuilder.Entity<Messages>().HasOne(m => m.Customer).WithMany(c => c.Messages).HasForeignKey(m => m.CustomerId).OnDelete(DeleteBehavior.ClientSetNull);

            // Orders
            modelBuilder.Entity<Orders>().HasOne(o => o.Contractor).WithMany(c => c.Orders).HasForeignKey(o => o.ContractorId);
            modelBuilder.Entity<Orders>().HasOne(o => o.Subscription).WithOne(s => s.Order).HasForeignKey<Orders>(o => o.SubscriptionId).OnDelete(DeleteBehavior.ClientSetNull);

            // ProductImages
            modelBuilder.Entity<ProductImages>().HasOne(p => p.Product).WithMany(p => p.ProductImages).HasForeignKey(p => p.ProductId);

            // Products
            modelBuilder.Entity<Products>().HasOne(p => p.Contractor).WithMany(c => c.Products).HasForeignKey(p => p.ContractorId);

            // RequestDetails
            modelBuilder.Entity<RequestDetails>().HasOne(r => r.Product).WithMany(p => p.RequestDetails).HasForeignKey(r => r.ProductId);
            modelBuilder.Entity<RequestDetails>().HasOne(r => r.Request).WithMany(r => r.RequestDetails).HasForeignKey(r => r.RequestId).OnDelete(DeleteBehavior.ClientSetNull);

            // Requests
            modelBuilder.Entity<Requests>().HasOne(r => r.Contractor).WithMany(c => c.Requests).HasForeignKey(r => r.ContractorId);
            modelBuilder.Entity<Requests>().HasOne(r => r.Customer).WithMany(c => c.Requests).HasForeignKey(r => r.CustomerId).OnDelete(DeleteBehavior.ClientSetNull);

            // Subscriptions

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Get the connection string from appsettings.json
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            // Configure your SQL Server connection string here
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}