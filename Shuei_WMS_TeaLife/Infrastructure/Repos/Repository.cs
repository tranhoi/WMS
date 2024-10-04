using Application.Services;
using Application.Services.Authen;
using Application.Services.Inbound;
using Application.Services.Suppliers;
using Application.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class Repository
    {
        public IProducts SProducts { get; set; }
        public IPermissions SPermissions { get; set; }
        public IPermissionTenant SPermissionTenant { get; set; }
        public IRoleToPermissions SRoleToPermissions { get; set; }
        public IRoleToPermissionTenant SRoleToPermissionTenant { get; set; }

        public IProductJanCodes SProductJanCodes { get; set; }

        public IVendors SVendors { get; set; }
        public IUserVendors SUserVendors { get; set; }
        public IVendorBillings SVendorBillings { get; set; }
        public IVendorBillingDetail SVendorBillingDetail { get; set; }


        public ILocations SLocations { get; set; }

        public IDevices SDevices { get; set; }

        public IBins SBins { get; set; }
        public IProductCategory SProductCategories { get; set; }
        public IUnits SUnits { get; set; }
        public ICurrency SCurrency { get; set; }
        public ICurrencyPairSetting SCurrencyPairSetting { get; set; }

        public ITenants STennats { get; set; }
        public IUserToTenant  SUserToTenant { get; set; }

        public ISuppliers  SSuppliers { get; set; }

        public IArrivalInstructions   SArrivalInstructions { get; set; }
        public IArrivalInstructionDetails  SArrivalInstructionDetails { get; set; }
        public IWarehousePutAway  SWarehousePutAways { get; set; }
        public IWarehousePutAwayLine  SWarehousePutAwayLines { get; set; }
        public IWarehousePutAwayStaging  SWarehousePutAwayStagings { get; set; }
        public IWarehouseReceiptOrder  SWarehouseReceiptOrders { get; set; }
        public IWarehouseReceiptOrderLine  SWarehouseReceiptOrderLines { get; set; }
        public IWarehouseReceiptStaging  SWarehouseReceiptStagings { get; set; }
        public IWarehouseTran    SWarehouseTrans { get; set; }
        public INumberSequences  SNumberSequences { get; set; }
        public IBatches SBatches{ get; set; }

        public Repository(IProducts sProduct = null, ILocations sLocations = null, IDevices sDevices = null
            , IProductJanCodes sProductJanCodes = null, IVendors sVendors = null, IBins sBins = null
            , IProductCategory sProductCategories = null, IUnits sUnits = null, IUserVendors sUserVendors = null
            , IVendorBillings sVendorBillings = null, IVendorBillingDetail sVendorBillingDetail = null
            , ICurrency sCurrencys = null, ICurrencyPairSetting sCurrencyPairSetting = null
            , IPermissions sPermissions = null, IPermissionTenant sPermissionTenant = null
            , IRoleToPermissions sRoleToPermissions = null, IRoleToPermissionTenant sRoleToPermissionTenant = null
            , ITenants sTennats = null, IUserToTenant sUserToTenant = null, ISuppliers iSuppliers = null
            , IArrivalInstructions sArrivalInstructions = null, IArrivalInstructionDetails sArrivalInstructionDetails = null
            , IWarehousePutAway sWarehousePutAways = null, IWarehousePutAwayLine sWarehousePutAwayLines = null
            , IWarehousePutAwayStaging sWarehousePutAwayStagings = null, IWarehouseReceiptOrder sWarehouseReceiptOrders = null
            , IWarehouseReceiptOrderLine sWarehouseReceiptOrderLines = null, IWarehouseReceiptStaging sWarehouseReceiptStagings = null
            , IWarehouseTran sWarehouseTrans = null, INumberSequences sNumberSequences = null, IBatches sBatches = null)
        {
            SProducts = sProduct;
            SLocations = sLocations;
            SDevices = sDevices;
            SProductJanCodes = sProductJanCodes;
            SVendors = sVendors;
            SBins = sBins;
            SProductCategories = sProductCategories;
            SUnits = sUnits;
            SUserVendors = sUserVendors;
            SVendorBillings = sVendorBillings;
            SVendorBillingDetail = sVendorBillingDetail;

            SCurrency = sCurrencys;
            SCurrencyPairSetting = sCurrencyPairSetting;
            SPermissions = sPermissions;
            SPermissionTenant = sPermissionTenant;
            SRoleToPermissions = sRoleToPermissions;
            SRoleToPermissionTenant = sRoleToPermissionTenant;
            STennats = sTennats;
            SUserToTenant = sUserToTenant;
            SSuppliers = iSuppliers;
            SArrivalInstructions = sArrivalInstructions;
            SArrivalInstructionDetails = sArrivalInstructionDetails;
            SWarehousePutAways = sWarehousePutAways;
            SWarehousePutAwayLines = sWarehousePutAwayLines;
            SWarehousePutAwayStagings = sWarehousePutAwayStagings;
            SWarehouseReceiptOrders = sWarehouseReceiptOrders;
            SWarehouseReceiptOrderLines = sWarehouseReceiptOrderLines;
            SWarehouseReceiptStagings = sWarehouseReceiptStagings;
            SWarehouseTrans = sWarehouseTrans;
            SNumberSequences = sNumberSequences;
            SBatches = sBatches;
        }
    }
}
