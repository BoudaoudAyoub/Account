using Account.Infrastructure;
using Account.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Account.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace Account.API.Infrastructure.Seed;

public static class SeedData
{
    public static void Seeder(this IServiceCollection service)
    {
        ServiceProvider serviceProvider = service.BuildServiceProvider();

        AccountDbContext accountDbContext = serviceProvider.GetRequiredService<AccountDbContext>();

        RelationalDatabaseCreator dbCreator = accountDbContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator ?? default!;

        if (dbCreator is null)
        {
            Log.Error("Database creator is not available.");
            return;
        }

        if (!dbCreator.Exists())
        {
            Log.Information("Started database migration...");
            accountDbContext.Database.Migrate();
        }
        try
        {
            // Check and add identity roles if not exits
            List<string> roles = Roles().Where(role => !accountDbContext.Roles.AsEnumerable().Any(e => e.Name!.Equals(role, StringComparison.OrdinalIgnoreCase))).ToList();

            if (roles.Count > 0)
            {
                accountDbContext.Roles.AddRange(roles.Select(role => new Microsoft.AspNetCore.Identity.IdentityRole()
                {
                    Name = role,
                    NormalizedName = role.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }));

                accountDbContext.SaveChanges();
            }

            // Check user and add it if not exits

            if (accountDbContext.Users.FirstOrDefault(user => user.UserName!.Contains(SystemConstants.ADMIN_USERNAME)) is null)
            {
                IdentityUser user = new()
                {
                    UserName = SystemConstants.ADMIN_USERNAME,
                    Email = SystemConstants.ADMIN_USERNAME,
                    NormalizedEmail = SystemConstants.ADMIN_USERNAME
                };

                UserManager<IdentityUser> userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                _ = userManager.CreateAsync(user, SystemConstants.PASSWORD).Result;

                string systemRoleId = accountDbContext.Roles.First(e => e.Name!.Contains(SystemConstants.SYSTEMADMIN)).Id;

                accountDbContext.UserRoles.Add(new()
                {
                    RoleId = systemRoleId,
                    UserId = user.Id
                });

                accountDbContext.ApplicationUsers.Add(new(user.Id, SystemConstants.SYSTEM_FIRSTNAME, SystemConstants.SYSTEM_LASTNAME)
                {
                    Creator = SystemConstants.ADMIN_USERNAME
                });

                accountDbContext.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            accountDbContext.Database.RollbackTransaction();
            throw new DomainException(ex.Message);
        }
    }

    private static List<string> Roles() => ["Admin", "SysAdmin", "User"];
}