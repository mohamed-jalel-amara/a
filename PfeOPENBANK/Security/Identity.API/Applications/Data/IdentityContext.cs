using Identity.API.Applications.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.API.Applications.Data
{
    public class IdentityContext : IdentityDbContext<UserModel> 
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<Entitlement> Entitlements { get; set; }
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserLogin<string>>(id => id.HasKey(k => k.UserId));
            modelBuilder.Entity<IdentityUserRole<string>>(ur => ur.HasKey(k => new
            {
                k.RoleId,
                k.UserId
            }));
            modelBuilder.Entity<IdentityUserToken<string>>(ut => ut.HasKey(k => new
            {
                k.UserId,
                k.LoginProvider,
                k.Name
            }));

            modelBuilder.Entity<UserModel>().ToTable("Users");
        }
    }

    public class IdentityContextFactory : IDesignTimeDbContextFactory<IdentityContext>
    {
        public IdentityContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=UserDb;User Id=sa;Password=SwN12345678@Pass;");

            return new IdentityContext(optionsBuilder.Options);
        }
    }
}
