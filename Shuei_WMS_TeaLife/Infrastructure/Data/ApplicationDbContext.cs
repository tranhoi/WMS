using Application.Models;
using Domain.Entity.authp.Commons;
using Domain.Entity.Common;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Domain.Entity.WMS.Authentication;
using Domain.Entity.WMS.Inbound;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option)
        {

        }

        public string GetConnectionString()
        {
            return this.Database.GetDbConnection().ConnectionString;
        }

        #region common
        public DbSet<Product> Products { get; set; }
        public DbSet<ApiCode> ApiCodes { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelMaster> ChannelMasters { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<CountryMaster> CountryMasters { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyPairSetting> CurrencyPairSettings { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderReturnItem> OrderReturnItems { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<ProductBundle> ProductBundles { get; set; }
        public DbSet<ProductJanCode> ProductJanCodes { get; set; }
        public DbSet<ProductStatus> ProductStatuses { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }

        public DbSet<SalesDatum> SalesDatums { get; set; }
        public DbSet<ShippingCountry> ShippingCountries { get; set; }
        public DbSet<SystemClassCompany> SystemClassCompanies { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<UserVendor> UserVendors { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<WarehouseUserSetting> WarehouseUserSettings { get; set; }

        public DbSet<VendorBilling> VendorBillings { get; set; }
        public DbSet<VendorBillingDetail> VendorBillingDetails { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ArrivalInstruction> ArrivalInstructions { get; set; }
        public DbSet<ArrivalInstructionDetail> ArrivalInstructionDetails { get; set; }
        #endregion

        #region authp
        public DbSet<RefreshTokenAuth> RefreshTokensAuthP { get; set; }
        public DbSet<RoleToPermissionAuth> RoleToPermissionsRefreshTokensAuthP { get; set; }
        public DbSet<RoleToPermissionsTenantAuth> RoleToPermissionsTenantsRefreshTokensAuthP { get; set; }
        public DbSet<TenantAuth> TenantAuth { get; set; }
        public DbSet<AuthUser> AuthUsers { get; set; }
        public DbSet<UserToRole> userToRoles { get; set; }
        #endregion

        #region WMS
        public DbSet<MstUserSetting> MstUserSettings { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<PermissionsTenant> PermissionsTenants { get; set; }
        public DbSet<RoleToPermission> RoleToPermissions { get; set; }
        public DbSet<RoleToPermissionTenant> RoleToPermissionTenants { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Bin> Bins { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserToTenant> UserToTenants { get; set; }

        public DbSet<WarehouseTran> WarehouseTrans { get; set; }
        public DbSet<WarehousePutAway> WarehousePutAways { get; set; }
        public DbSet<WarehousePutAwayLine> WarehousePutAwayLines { get; set; }
        public DbSet<WarehousePutAwayStaging> WarehousePutAwayStagings { get; set; }
        public DbSet<WarehouseReceiptOrder> WarehouseReceiptOrders { get; set; }
        public DbSet<WarehouseReceiptOrderLine> WarehouseReceiptOrderLines { get; set; }
        public DbSet<WarehouseReceiptStaging> WarehouseReceiptStagings { get; set; }
        public DbSet<NumberSequences>  SequencesNumber { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region DB Chinh ko migration
            // Ánh xạ bảng "Orders" tới schema "sales"
            //modelBuilder.Entity<PermissionsListModel>().ToTable("PermissionsListModels", "dbo", x => x.ExcludeFromMigrations());//ko cho migration cac bang hien co cua FBT_DEV

            modelBuilder.Entity<AuthUser>()
             .ToTable("AuthUser", "authp", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RefreshTokenAuth>()
                .ToTable("AuthUsers", "authp", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RoleToPermissionAuth>()
               .ToTable("RoleToPermission", "authp", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RoleToPermissionsTenantAuth>()
               .ToTable("RoleToPermissionsTenant", "authp", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<TenantAuth>()
               .ToTable("Tenants", "authp", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<UserToRole>()
               .ToTable("UserToRole", "authp", x => x.ExcludeFromMigrations());
            #endregion

            modelBuilder.Entity<MstUserSetting>()
             .ToTable("MstUserSetting", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RefreshTokens>()
             .ToTable("RefreshTokens", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Permissions>()
              .ToTable("Permissions", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<PermissionsTenant>()
             .ToTable("PermissionsTenant", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RoleToPermission>()
              .ToTable("RoleToPermission", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RoleToPermissionTenant>()
             .ToTable("RoleToPermissionTenant", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Location>()
             .ToTable("Locations", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Device>()
             .ToTable("Devices", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Bin>()
             .ToTable("Bins", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductCategory>()
             .ToTable("ProductCategories", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Unit>()
             .ToTable("Units", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserToTenant>()
             .ToTable("UserToTenant", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WarehouseTran>()
            .ToTable("WarehouseTrans", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehousePutAway>()
            .ToTable("WarehousePutAways", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehousePutAwayLine>()
            .ToTable("WarehousePutAwayLines", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehousePutAwayStaging>()
            .ToTable("WarehousePutAwayStaging", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehouseReceiptOrder>()
            .ToTable("WarehouseReceiptOrder", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehouseReceiptOrderLine>()
            .ToTable("WarehouseReceiptOrderLine", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WarehouseReceiptStaging>()
            .ToTable("WarehouseReceptStaging", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<NumberSequences>()
            .ToTable("NumberSequences", "wms", x => x.ExcludeFromMigrations());
            base.OnModelCreating(modelBuilder);

            //override lai cac bang identity
            modelBuilder.Entity<IdentityRoleClaim<string>>()
             .ToTable("AspNetRoleClaims", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityRole>()
             .ToTable("AspNetRoles", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserClaim<string>>()
             .ToTable("AspNetUserClaims", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserLogin<string>>()
             .ToTable("AspNetUserLogins", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserRole<string>>()
             .ToTable("AspNetUserRoles", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<ApplicationUser>()
             .ToTable("AspNetUsers", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserToken<string>>()
             .ToTable("AspNetUserTokens", "wms", x => x.ExcludeFromMigrations());

            // Ghi đè bảng __EFMigrationsHistory
            //modelBuilder.HasAnnotation("Relational:Schema", "wms", x => x.ExcludeFromMigrations());  // Đặt schema tùy chỉnh
            // modelBuilder.HasAnnotation("Relational:HistoryTableName", "__EFMigrationsHistoryWMS");  // Đặt tên bảng tùy chỉnh
        }
    }
}
