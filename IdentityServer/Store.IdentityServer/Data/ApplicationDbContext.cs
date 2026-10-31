using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System;
namespace Store.IdentityServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // KVKK: TCKimlik alanı veritabanına şifreli yazılıp, şifresi çözülerek okunacak
            builder.Entity<ApplicationUser>()
                .Property(u => u.TCKimlik)
                .HasConversion(
                    v => Store.IdentityServer.Security.EncryptionHelper.Encrypt(v),
                    v => Store.IdentityServer.Security.EncryptionHelper.Decrypt(v)
                );
        }
    }
}
