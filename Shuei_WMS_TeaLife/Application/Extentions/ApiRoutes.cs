namespace Application.Extentions
{
    public static class ApiRoutes
    {
        /// <summary>
        /// httpGet.
        /// </summary>
        public const string GetAll = "";
        /// <summary>
        /// httpget.
        /// </summary>
        public const string GetById = "{id}";
        /// <summary>
        /// httppost.
        /// </summary>
        public const string Update = "update";
        /// <summary>
        /// httppost.
        /// </summary>
        public const string Insert = "insert";
        public const string Delete = "delete";
        public const string AddRange = "AddRange";
        public const string DeleteRange = "DeleteRange";
        public const string DeleteById = "DeleteById/{id}";

        public static class Identity
        {
            public const string BasePath = "api/account";
            public const string Login = "identity/loginasync";
            public const string LoginHt = "identity/loginht";
            public const string CreateAccount = "identity/create";
            public const string RefreshToken = "identity/refresh-token";
            public const string CreateRole = "identity/role/create";
            public const string RoleList = "identity/role/list";
            public const string CreateSuperAdminAccount = "identity/setting";
            public const string UserWithRole = "identity/user-with-role";
            public const string ChangePassword = "identity/change-pass";
            public const string ChangeUserRole = "identity/change-role";
            public const string AssignUserRole = "identity/assign_user_role";
            public const string DeleteUser = "identity/delete-user";
            public const string DeleteUserRole = "identity/delete-user-role";
            public const string UpdateRole = "identity/update-role-name";
            public const string UpdateUserInfo = "identity/update-user-info";
            public const string UserGetById = "identity/{id}";
            public const string DeleteRole = "identity/role/delete";
            public const string UserGetByEmail = "identity/UserGetByEmail/{email}";
            public const string RoleGetById = "identity/Role/{id}";
            public const string GetReportBase64 = "identity/GetReportBase64/{id}";
            public const string GeneratePdf = "identity/GeneratePdf";
            public const string GetLabelById = "identity/GetLabelById/{id}";
            public const string GetLabelsAll = "identity/GetLabelsAll";
            public const string JobApi = "JobApi";
        }
        public static class Permissions
        {
            public const string BasePath = "api/Permissions";
            public const string GetAllPermissionWithAssignedRole = "Get-All-Permission-With-Assigned-To-Role";
            public const string AddOrEdit = "add-or-edit";
        }
        public static class PermissionTenant
        {
            public const string BasePath = "api/PermissionTenant";
        }
        public static class RoleToPermissions
        {
            public const string BasePath = "api/RoleToPermissions";
            public const string GetByPermissionId = "GetByPermissionId/{id}";
        }
        public static class RoleToPermissionTenant
        {
            public const string BasePath = "api/RoleToPermissionsTenant";
        }

        public static class Product
        {
            public const string BasePath = "api/Products";
            public const string GetFillter = "GetFillter";
            public const string UploadProductImage = "UploadProductImage";
            public const string GetProductListAsync = "get-product-list";
            public const string GetByProductCodeAsync = "get-by-product-code";
            public const string SearchByProductCodeAsync = "search-by-product-code";
        }
        public static class ProductCategories
        {
            public const string BasePath = "api/ProductCategories";
        }
        public static class Unit
        {
            public const string BasePath = "api/Units";
        }

        public static class Locations
        {
            public const string BasePath = "api/Locations";
            public const string DeleteLocation = "deletelocation";
        }
        public static class Bins
        {
            public const string BasePath = "api/Bins";
            public const string GetByLocationId = "GetByLocationId/{locationId}";
            public const string AddOrUpdate = "AddOrUpdate";
            public const string GetLabelById = "GetLabelById/{id}";
            public const string GetLabelByLocationId = "GetLabelByLocationId/{locationId}";
        }
        public static class ProductJanCodes
        {
            public const string BasePath = "api/ProductJanCodes";
            public const string GetByProductId = "GetByProductId/{productId}";
            public const string AddOrUpdateAsync = "add-or-update";
        }
        public static class Vendors
        {
            public const string BasePath = "api/Vendors";

        }
        public static class VendorBillings
        {
            public const string BasePath = "api/VendorBillings";

        }
        public static class VendorBillingDetail
        {
            public const string BasePath = "api/VendorBillingDetail";

        }
        public static class UserVendors
        {
            public const string BasePath = "api/UserVendors";

        }

        public static class Devices
        {
            public const string BasePath = "api/Devices";
        }

        public static class Currency
        {
            public const string BasePath = "api/Currency";
        }

        public static class CurrencyPairSetting
        {
            public const string BasePath = "api/CurrencyPairSetting";
        }

        public static class Tenants
        {
            public const string BasePath = "api/Tenants";

        }
        public static class UserToTenant
        {
            public const string BasePath = "api/UserToTenant";
            public const string GetByUserId = "GetByUserId/{userId}";
            public const string GetUsersAsync = "get-users";
        }

        public static class Suppliers
        {
            public const string BasePath = "api/Suppliers";
            public const string GetSupplierWithTenantAsync = "GetSupplierWithTenantAsync";
        }

        //inbound
        public static class ArrivalInstructions
        {
            public const string BasePath = "api/ArrivalInstructions";
        }
        public static class ArrivalInstructionDetails
        {
            public const string BasePath = "api/ArrivalInstructionDetails";
        }
        public static class WarehouseTran
        {
            public const string BasePath = "api/WarehouseTran";
        }
        public static class WarehousePutAway
        {
            public const string BasePath = "api/WarehousePutAway";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
            public const string InsertWarehousePutAwayOrder = "insert-warehouse-put-away-order";
            public const string GetPutAwayAsync = "get-put-away";
            public const string SyncHTData = "sync-ht-data";
            public const string AdjustActionPutAway = "adjust-action-put-away";
        }
        public static class WarehousePutAwayLine
        {
            public const string BasePath = "api/WarehousePutAwayLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
            public const string GetLabelById = "GetLabelById/{id}";
            public const string GetLabelByPutAwayNo = "GetLabelByPutAwayNo/{putAwayNo}";
            public const string GetByPutAwayLineIdAsync = "get-by-putAwayLineid";
        }
        public static class WarehousePutAwayStaging
        {
            public const string BasePath = "api/WarehousePutAwayStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
            public const string GetByPutAwayLineIdAsync = "get-by-putAwayLineid";
        }
        public static class WarehouseReceiptOrder
        {
            public const string BasePath = "api/WarehouseReceiptOrder";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
            public const string InsertWarehouseReceiptOrder = "insert-warehouse-receipt";
            public const string UpdateWarehouseReceiptOrder = "update-warehouse-receipt";
            public const string GetReceiptOrderAsync = "get-receipt-order/{receiptNo}";
            public const string GetReceiptOrderListAsync = "get-receipt-order-list";
            public const string SyncHTData = "sync-ht-data";
            public const string AdjustActionReceiptOrder = "adjust-action-receipt-order";
            public const string CreateLineFromArrivalNo = "CreateLineFromArrivalNo";
            
        }
        public static class WarehouseReceiptOrderLine
        {
            public const string BasePath = "api/WarehouseReceiptOrderLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
        }
        public static class WarehouseReceiptStaging
        {
            public const string BasePath = "api/WarehouseReceiptStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
            public const string GetByReceiptLineIdAsync = "GetByReceiptLineIdAsync/{receiptLineId}";
        }

        public static class NumberSequences
        {
            public const string BasePath = "api/SequenceNumbers";
            public const string GetNumberSequenceByType = "GetNumberSequenceByType/{type}";
            public const string IncreaseNumberSequenceByType = "IncreaseNumberSequenceByType/{type}";
        }

        public static class Reports
        {
            public const string BasePath = "api/Reports";
            public const string GetReportBase64 = "GetReportBase64";
        }

        public static class Batches
        {
            public const string BasePath = "api/Batches";
        }

        public static class ShippingBoxs
        {
            public const string BasePath = "api/ShippingBoxs";
            public const string GetShippingBoxListAsync = "api/GetShippingBoxListAsync";
        }

        public static class WarehouseShipment
        {
            public const string BasePath = "api/WarehouseShipment";
            public const string GetAsync = "GetAsync/{id}";
            public const string SearchAsync = "SearchAsync";
            public const string CreateAsync = "CreateAsync";
            public const string UpdateAsync = "UpdateAsync";
            public const string DeleteAsync = "DeleteAsync/{id}";
            public const string ConfirmShipmentAsync = "ConfirmShipmentAsync/{id}";
            public const string CreatePickingAsync = "CreatePickingAsync";
            public const string CheckMultipleShipmentCreatePicking = "CheckMultipleShipmentCreatePicking";

        }
        public static class WarehouseShipmentLine
        {
            public const string BasePath = "api/WarehouseShipmentLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{shipmentNo}";
        }
        public static class WarehousePickingList
        {
            public const string BasePath = "api/WarehousePickingList";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{pickNo}";
            public const string GetWarehousePickingDTOAsync = "GetWarehousePickingDTOAsync";
            public const string DeletePickingAsync = "DeletePickingAsync/{pickNo}";
            public const string CompletePickingAsync = "CompletePickingAsync/{pickNo}";
            public const string SyncToHTAsync = "SyncToHTAsync";
        }
        public static class WarehousePickingLine
        {
            public const string BasePath = "api/WarehousePickingLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{pickNo}";
            public const string GetPickingLineDTOAsync = "GetPickingLineDTOAsync/{pickNo}";
            public const string GetShipmentsByPickAsync = "GetShipmentsByPickAsync/{pickNo}";
            public const string UpdateWarehousePickingLinesAsync = "UpdateWarehousePickingLinesAsync";
        }
        public static class WarehousePickingStaging
        {
            public const string BasePath = "api/WarehousePickingStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{pickNo}";
        }
        public static class PackingList
        {
            public const string BasePath = "api/PackingList";
            public const string GetDataMasterAsync = "GetDataMasterAsync";
            public const string UpdatePackedQtyAsync = "UpdatePackedQtyAsync";
            public const string CompletePackingAsync = "CompletePackingAsync";
        }
        public static class ShippingBox
        {
            public const string BasePath = "api/ShippingBox";
        }
        public static class ShippingCarier
        {
            public const string BasePath = "api/ShippingCarrier";
        }

        public static class Categories
        {
            public const string BasePath = "api/categories";
            public const string GetUsers = "api/categories/users";
            public const string GetBinByLocationId = "api/categories/bin-by-locationid";
        }

        public static class ReturnOrder
        {
            public const string BasePath = "api/return-order";
            public const string GetAllReturnOrdersAsync = "get-all-return-orders";
            public const string GetReturnOrderByReturnNoAsync = "get-return-order-by-return-no";
            public const string InsertReturnOrderAsync = "insert-return-order";
            public const string UpdateReturnOrderAsync = "update-return-order";
            public const string DeleteReturnOrderAsync = "delete-return-order/{id}";
        }

        public static class InventTransfer
        {
            public const string BasePath = "api/InventTransfer";
            public const string GetAllDTO = "GetAllDTO";
            public const string GetByIdDTO = "GetByIdDTO/{id}";
            public const string GetByTransferNoDTO = "GetByTransferNoDTO/{transferNo}";
        }

        public static class InventTransferLines
        {
            public const string BasePath = "api/InventTransferLines";
        }
    }
}
