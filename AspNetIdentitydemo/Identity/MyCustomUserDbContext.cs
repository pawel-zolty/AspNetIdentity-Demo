using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetIdentitydemo.Identity
{
    public class MyCustomUserDbContext : IdentityDbContext<MyCustomUser>
    {
        public MyCustomUserDbContext(DbContextOptions<MyCustomUserDbContext> options) : base(options)
        {
        }

        public DbSet<Organization> Organizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MyCustomUser>(user => user.HasIndex(x => new { x.OrganizationId, x.Id }));

            builder.Entity<Organization>(org =>
            {
                org.ToTable("Organizations");
                org.HasKey(x => x.Id);
                org.Property(o => o.ExternalId).IsRequired();
                org.HasIndex(o => o.ExternalId).IsUnique();

                org.HasMany<MyCustomUser>().WithOne()
                    .HasForeignKey(user => user.OrganizationId)
                    .IsRequired();
            });
        }
    }
}