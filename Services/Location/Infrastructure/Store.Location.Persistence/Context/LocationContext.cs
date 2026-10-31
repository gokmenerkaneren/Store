using Microsoft.EntityFrameworkCore;
using Store.Shared.Enums;
using System;
using Store.Shared.Enums;
using System.Collections.Generic;
using Store.Shared.Enums;
using System.Linq;
using Store.Shared.Enums;
using System.Net;
using Store.Shared.Enums;
using System.Text;
using Store.Shared.Enums;
using System.Threading.Tasks;
using Store.Shared.Enums;
using Npgsql.PostgresTypes;
using Store.Shared.Enums;
using Store.Location.Domain.Entities;
using Store.Shared.Enums;

namespace Store.Location.Persistence.Context
{
    public class LocationContext : DbContext
    {
        public LocationContext()
        {
        }

        public LocationContext(DbContextOptions<LocationContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connStr = Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSqlConnection")
                    ?? "Server=localhost;Port=5432;Database=StoreLocationDb;Username=postgres;Password=__YOUR_DB_PASSWORD__;";
                optionsBuilder.UseNpgsql(connStr, pgOptions =>
                {
                    pgOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorCodesToAdd: null);
                });
            }
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchAddress> BranchAddresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Dealer> Dealers { get; set; }
        public DbSet<DealerAddress> DealerAddresses { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseAddress> WarehouseAddresses { get; set; }
        public DbSet<Mesajlar> Mesajlar { get; set; }
        public DbSet<BultenAboneligi> BultenAboneligi { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // TPC (TABLE-PER-CONCRETE-TYPE) MAPPING
            // EF Core model hiyerarşisi döngüden ÖNCE tanımlanmalı.
            // ==========================================
            modelBuilder.Entity<AddressBase>().UseTpcMappingStrategy();

            // ==========================================
            // GLOBAL QUERY FILTER (Soft Delete)
            // Hiyerarşik yapılarda (AddressBase türevleri) query filter YALNIZCA kök entity (AddressBase)'e uygulanmalıdır.
            // EF Core bu filtreyi türetilmiş sınıflara (BranchAddress, UserAddress, DealerAddress) otomatik kalıtır.
            // ==========================================
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Eğer entity AddressBase'den türiyorsa ve kendisi AddressBase değilse atla (kök sınıfa uygulayacağız)
                if (typeof(AddressBase).IsAssignableFrom(entityType.ClrType) && entityType.ClrType != typeof(AddressBase))
                {
                    continue;
                }

                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }

            // ==========================================
            // ENUM DÖNÜŞÜMÜ
            // AddressType enum'u veritabanında integer olarak saklanır.
            // ==========================================
            modelBuilder.Entity<DealerAddress>()
                .Property(d => d.AddressType)
                .HasConversion<int>();

            // ==========================================
            // KVKK ŞİFRELEME (UserAddress.IdentityNumber)
            // ==========================================
            modelBuilder.Entity<UserAddress>()
                .Property(u => u.IdentityNumber)
                .HasConversion(new Converters.EncryptedStringConverter())
                .HasMaxLength(256);

            // ==========================================
            // CASCADE DELETE KURALLARI
            // ==========================================

            // DealerAddress → Cascade: Bayi silinince adresleri de silinir
            modelBuilder.Entity<DealerAddress>()
                .HasOne(d => d.Dealer)
                .WithMany(d => d.Addresses)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Location ilişkileri (AddressBase türevleri için)
            modelBuilder.Entity<AddressBase>()
                .HasOne(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AddressBase>()
                .HasOne(a => a.City)
                .WithMany()
                .HasForeignKey(a => a.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AddressBase>()
                .HasOne(a => a.District)
                .WithMany()
                .HasForeignKey(a => a.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AddressBase>()
                .HasOne(a => a.Neighborhood)
                .WithMany()
                .HasForeignKey(a => a.NeighborhoodId)
                .OnDelete(DeleteBehavior.Restrict);

            // Warehouse → ContactInfo
            modelBuilder.Entity<Warehouse>()
                .HasOne(w => w.ContactInfo)
                .WithMany()
                .HasForeignKey(w => w.ContactInfoId)
                .OnDelete(DeleteBehavior.Restrict);

            // WarehouseAddress → Warehouse
            modelBuilder.Entity<WarehouseAddress>()
                .HasOne(w => w.Warehouse)
                .WithMany()
                .HasForeignKey(w => w.WarehouseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Branch → ContactInfo
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.ContactInfo)
                .WithMany()
                .HasForeignKey(b => b.ContactInfoId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(Type entityType)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var falseConstant = System.Linq.Expressions.Expression.Constant(false);
            var compare = System.Linq.Expressions.Expression.Equal(property, falseConstant);

            return System.Linq.Expressions.Expression.Lambda(compare, parameter);
        }
    }
}
