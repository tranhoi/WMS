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

        public static class Identity
        {
            public const string BasePath = "api/account";
            public const string Login = "identity/login";
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

        }

        public static class Suppliers
        {
            public const string BasePath = "api/Suppliers";
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
        }
        public static class WarehousePutAwayLine
        {
            public const string BasePath = "api/WarehousePutAwayLine";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
        }
        public static class WarehousePutAwayStaging
        {
            public const string BasePath = "api/WarehousePutAwayStaging";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{putAwayNo}";
        }
        public static class WarehouseReceiptOrder
        {
            public const string BasePath = "api/WarehouseReceiptOrder";
            public const string GetByMasterCodeAsync = "GetByMasterCodeAsync/{receiptNo}";
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
        }

        public static class NumberSequences
        {
            public const string BasePath = "api/SequenceNumbers";
        }

        public static class Reports
        {
            public const string BasePath = "api/Reports";
            public const string GetReportBase64 = "GetReportBase64";
        }
    }
}
