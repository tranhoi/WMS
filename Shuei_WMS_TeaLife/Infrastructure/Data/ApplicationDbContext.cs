using Application.Models;
using Domain.Entity.authp.Commons;
using Domain.Entity.Common;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Domain.Entity.WMS.Authentication;
using Domain.Entity.WMS.Inbound;
using Domain.Entity.WMS.Outbound;
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
        public DbSet<ProductCategory> ProductCategories { get; set; }
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
        public DbSet<TenantAuth> TenantAuth { get; set; }
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
        //public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UserToTenant> UserToTenants { get; set; }

        public DbSet<WarehouseTran> WarehouseTrans { get; set; }
        public DbSet<WarehousePutAway> WarehousePutAways { get; set; }
        public DbSet<WarehousePutAwayLine> WarehousePutAwayLines { get; set; }
        public DbSet<WarehousePutAwayStaging> WarehousePutAwayStagings { get; set; }
        public DbSet<WarehouseReceiptOrder> WarehouseReceiptOrders { get; set; }
        public DbSet<WarehouseReceiptOrderLine> WarehouseReceiptOrderLines { get; set; }
        public DbSet<WarehouseReceiptStaging> WarehouseReceiptStagings { get; set; }
        public DbSet<NumberSequences> SequencesNumber { get; set; }
        public DbSet<Batches> Batches { get; set; }
        public DbSet<LogTime> LogTimes { get; set; }

        #region Outbound
        public DbSet<ReturnOrder> ReturnOrders { get; set; }
        public DbSet<ReturnOrderLine> ReturnOrderLines { get; set; }
        public DbSet<ShippingBox> ShippingBoxes { get; set; }
        public DbSet<ShippingCarrier> ShippingCarriers { get; set; }
        public DbSet<WarehousePackingLine> WarehousePackingLines { get; set; }
        public DbSet<WarehousePackingList> WarehousePackingLists { get; set; }
        public DbSet<WarehousePickingLine> WarehousePickingLines { get; set; }
        public DbSet<WarehousePickingList> WarehousePickingLists { get; set; }
        public DbSet<WarehousePickingStaging> WarehousePickingStagings { get; set; }
        public DbSet<WarehouseShipment> WarehouseShipments { get; set; }
        public DbSet<WarehouseShipmentLine> WarehouseShipmentLines { get; set; }
        #endregion

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region DB Chinh ko migration
            // Ánh xạ bảng "Orders" tới schema "sales"
            //modelBuilder.Entity<PermissionsListModel>().ToTable("PermissionsListModels", "dbo", x => x.ExcludeFromMigrations());//ko cho migration cac bang hien co cua FBT_DEV

            modelBuilder.Entity<TenantAuth>()
               .ToTable("Tenants", "authp", x => x.ExcludeFromMigrations());
            #endregion

            #region WMS migration
            //   modelBuilder.Entity<MstUserSetting>()
            //   .ToTable("MstUserSetting", "wms");
            //   modelBuilder.Entity<RefreshTokens>()
            //    .ToTable("RefreshTokens", "wms");
            //   modelBuilder.Entity<Permissions>()
            //     .ToTable("Permissions", "wms");
            //   modelBuilder.Entity<PermissionsTenant>()
            //    .ToTable("PermissionsTenant", "wms");
            //   modelBuilder.Entity<RoleToPermission>()
            //     .ToTable("RoleToPermission", "wms");
            //   modelBuilder.Entity<RoleToPermissionTenant>()
            //    .ToTable("RoleToPermissionTenant", "wms");

            //   modelBuilder.Entity<Location>()
            //    .ToTable("Locations", "wms");
            //   modelBuilder.Entity<Device>()
            //    .ToTable("Devices", "wms");
            //   modelBuilder.Entity<Bin>().ToTable("Bins", "wms");

            //   modelBuilder.Entity<Unit>()
            //    .ToTable("Units", "wms");


            //   modelBuilder.Entity<UserToTenant>()
            //    .ToTable("UserToTenant", "wms");


            //   modelBuilder.Entity<WarehouseTran>()
            //   .ToTable("WarehouseTrans", "wms");

            //   modelBuilder.Entity<WarehousePutAway>()
            //   .ToTable("WarehousePutAways", "wms");

            //   modelBuilder.Entity<WarehousePutAwayLine>()
            //   .ToTable("WarehousePutAwayLines", "wms");

            //   modelBuilder.Entity<WarehousePutAwayStaging>()
            //   .ToTable("WarehousePutAwayStaging", "wms");

            //   modelBuilder.Entity<WarehouseReceiptOrder>()
            //   .ToTable("WarehouseReceiptOrder", "wms");

            //   modelBuilder.Entity<WarehouseReceiptOrderLine>()
            //   .ToTable("WarehouseReceiptOrderLine", "wms");

            //   modelBuilder.Entity<WarehouseReceiptStaging>()
            //   .ToTable("WarehouseReceiptStaging", "wms");

            //   modelBuilder.Entity<NumberSequences>()
            //   .ToTable("NumberSequences", "wms");

            //   modelBuilder.Entity<Batches>()
            //   .ToTable("Batches", "wms");


            //   modelBuilder.Entity<ReturnOrder>()
            //   .ToTable("ReturnOrders", "wms");

            //   modelBuilder.Entity<ReturnOrderLine>()
            //.ToTable("ReturnOrderLines", "wms");

            //   modelBuilder.Entity<ShippingBox>()
            //.ToTable("ShippingBoxes", "wms");

            //   modelBuilder.Entity<ShippingCarrier>()
            //.ToTable("ShippingCarriers", "wms");

            //   modelBuilder.Entity<WarehousePackingLine>()
            //.ToTable("WarehousePackingLines", "wms");

            //   modelBuilder.Entity<WarehousePackingList>()
            //.ToTable("WarehousePackingList", "wms");

            //   modelBuilder.Entity<WarehousePickingLine>()
            //.ToTable("WarehousePickingLines", "wms");

            //   modelBuilder.Entity<WarehousePickingList>()
            //.ToTable("WarehousePickingList", "wms");

            //   modelBuilder.Entity<WarehousePickingStaging>()
            //.ToTable("WarehousePickingStaging", "wms");

            //   modelBuilder.Entity<WarehouseShipment>()
            //.ToTable("WarehouseShipments", "wms");

            //   modelBuilder.Entity<WarehouseShipmentLine>()
            //.ToTable("WarehouseShipmentLines", "wms");

            #endregion

            #region WMS don't migration
            modelBuilder.Entity<MstUserSetting>().ToTable("MstUserSetting", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RefreshTokens>().ToTable("RefreshTokens", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Permissions>().ToTable("Permissions", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<PermissionsTenant>().ToTable("PermissionsTenant", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RoleToPermission>().ToTable("RoleToPermission", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<RoleToPermissionTenant>().ToTable("RoleToPermissionTenant", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Location>().ToTable("Locations", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Device>().ToTable("Devices", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Bin>().ToTable("Bins", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Unit>().ToTable("Units", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<UserToTenant>().ToTable("UserToTenant", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehouseTran>().ToTable("WarehouseTrans", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePutAway>().ToTable("WarehousePutAways", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePutAwayLine>().ToTable("WarehousePutAwayLines", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePutAwayStaging>().ToTable("WarehousePutAwayStaging", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehouseReceiptOrder>().ToTable("WarehouseReceiptOrder", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehouseReceiptOrderLine>().ToTable("WarehouseReceiptOrderLine", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehouseReceiptStaging>().ToTable("WarehouseReceiptStaging", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<NumberSequences>().ToTable("NumberSequences", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<Batches>().ToTable("Batches", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<ReturnOrder>().ToTable("ReturnOrders", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<ReturnOrderLine>().ToTable("ReturnOrderLines", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<ShippingBox>().ToTable("ShippingBoxes", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<ShippingCarrier>().ToTable("ShippingCarriers", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePackingLine>().ToTable("WarehousePackingLines", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePackingList>().ToTable("WarehousePackingList", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePickingLine>().ToTable("WarehousePickingLines", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePickingList>().ToTable("WarehousePickingList", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehousePickingStaging>().ToTable("WarehousePickingStaging", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehouseShipment>().ToTable("WarehouseShipments", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<WarehouseShipmentLine>().ToTable("WarehouseShipmentLines", "wms", x => x.ExcludeFromMigrations());

            modelBuilder.Entity<LogTime>().ToTable("LogTime", "wms", x => x.ExcludeFromMigrations());
            #endregion
            //override lai cac bang identity
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "wms", x => x.ExcludeFromMigrations());
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "wms", x => x.ExcludeFromMigrations());
            
            modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers", "wms", x => x.ExcludeFromMigrations());

            // Ghi đè bảng __EFMigrationsHistory
            //modelBuilder.HasAnnotation("Relational:Schema", "wms", x => x.ExcludeFromMigrations());  // Đặt schema tùy chỉnh
            // modelBuilder.HasAnnotation("Relational:HistoryTableName", "__EFMigrationsHistoryWMS");  // Đặt tên bảng tùy chỉnh
        }
    }
}
