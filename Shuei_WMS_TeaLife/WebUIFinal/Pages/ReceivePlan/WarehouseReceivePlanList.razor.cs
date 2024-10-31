using Microsoft.JSInterop;
using WebUIFinal.TemplateHtmlPrintLabel;
using static Application.Extentions.ApiRoutes;
using ReceivePlanModel = FBT.ShareModels.Entities.ArrivalInstruction;
using SupplierModel = FBT.ShareModels.Entities.Supplier;
namespace WebUIFinal.Pages.ReceivePlan
{
    public partial class WarehouseReceivePlanList
    {
        List<ReceivePlanModel> _dataGrid = new();
       
        RadzenDataGrid<ReceivePlanModel>? _receivePlanGrid;
        private List<ReceivePlanModel> _filteredModel = new List<ReceivePlanModel>();
        List<TenantAuth> tenants = new (); // Fix: Initialize the list with the 'new' keyword
        private List<SupplierModel> suppliers = new();
        private List<CompanyTenant> companys = new();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RefreshDataAsync();
            await GetSupplierAsync();
       //     await GetCompanyAsync();
            _filteredReceivePlanItems = new List<ReceivePlanModel>(_dataGrid);
        }

       // private async Task GetCompanyAsync() => companys = (await _com.GetAllAsync()).Data.ToList();
        private async Task GetSupplierAsync() => suppliers = (await _suppliersServices.GetAllAsync()).Data.ToList();
        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _warehouseReceivePlan.GetAllAsync();

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.ToString(),
                    });
                    return;
                  
                }

                _dataGrid = res.Data.ToList();
                _filteredReceivePlanItems = _dataGrid;

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 5000
                });
                return;
            }
        }
    }
}
