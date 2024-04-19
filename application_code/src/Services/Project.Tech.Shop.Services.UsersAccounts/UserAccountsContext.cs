using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.Tech.Shop.Services.Common;
using Project.Tech.Shop.Services.UsersAccounts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tech.Shop.Services.UsersAccounts
{
    public class UserAccountsContext : DbContext, IUnitOfWork
    {
        private readonly ILogger<UserAccountsContext>? _logger;
        private const string SchemaName = "UserAccounts";

        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SecuritySettings> SecuritySettings { get; set; }
        public DbSet<UserPreferences> Preferences { get; set; }
        public DbSet<AccountActivity> Activities { get; set; }

        public UserAccountsContext()
        {
        }

        public UserAccountsContext(DbContextOptions<UserAccountsContext> options) : base(options)
        {
        }

        public UserAccountsContext(DbContextOptions<UserAccountsContext> options, ILogger<UserAccountsContext> logger) : base(options)
        {
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Database=dev-tech-shop;Port=5432;User Id=dts-postgres;Password=X1B2#WXYZ123a;",
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", SchemaName));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");
            modelBuilder.HasDefaultSchema(SchemaName);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(u => u.UserId)
                    .HasColumnType("uuid")
                    .HasDefaultValueSql("uuid_generate_v4()")
                    .IsRequired();
                entity.HasIndex(u => u.Username)
                    .IsUnique();
                entity.HasIndex(u => u.Email)
                    .IsUnique();
                entity.HasMany(u => u.Addresses)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId);
                entity.HasMany(u => u.Roles)
                    .WithMany(r => r.Users)
                    .UsingEntity(j => j.ToTable("UserRoles"));
                entity.HasOne(u => u.SecuritySettings)
                    .WithOne(s => s.User)
                    .HasForeignKey<SecuritySettings>(s => s.UserId);
                entity.HasOne(u => u.Preferences)
                    .WithOne(p => p.User)
                    .HasForeignKey<UserPreferences>(p => p.UserId);
                entity.HasMany(u => u.Activities)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId);
            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Addresses");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasMany(r => r.Users)
                    .WithMany(u => u.Roles);
            });

            modelBuilder.Entity<SecuritySettings>(entity =>
            {
                entity.ToTable("SecuritySettings");
            });

            modelBuilder.Entity<UserPreferences>(entity =>
            {
                entity.ToTable("UserPreferences");
            });

            modelBuilder.Entity<AccountActivity>(entity =>
            {
                entity.ToTable("AccountActivities");
                entity.Property(a => a.Timestamp)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .IsRequired();
            });
        }

        public async Task<UnitResult<UserDbErrorReason>> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await base.SaveChangesAsync(cancellationToken);
                return UnitResult.Success<UserDbErrorReason>();
            }
            catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            {
                _logger?.LogError("Error occurred during the entity update, the entity is not unique, exception: {Exception}", ex.ToString());
                return UnitResult.Failure(UserDbErrorReason.NotUnique);
            }
        }
    }
}
