using Microsoft.EntityFrameworkCore;
using Account.Domain.Aggregates.UserAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Account.Infrastructure.EntityTypeConfigurations.UserConfigurations;

public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(user => user.ID);
        builder.Property(user => user.Creator).IsRequired();
        builder.Property(user => user.LastName).IsRequired();
        builder.Property(user => user.FirstName).IsRequired();

        builder.HasOne(user => user.IdentityUser)
               .WithOne()
               .HasForeignKey<ApplicationUser>(user => user.ID);
    }
}