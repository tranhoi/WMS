using Radzen;
using Radzen.Blazor;
using SupplierTenantDTOEntity = Application.DTOs.SupplierTenantDTO;
using SupplierEntity = Domain.Entity.Commons.Supplier;

namespace WebUIFinal.Pages.Supplier
{
    public partial class SupplierMaster
    {
        List<SupplierTenantDTOEntity> _dataGrid = null;
        RadzenDataGrid<SupplierTenantDTOEntity> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/detailsupplier/{_localizerCommon["Detail.Create"]} {_localizer["Supplier"]}");

        async Task EditItemAsync(int id) => _navigation.NavigateTo($"/detailsupplier/{_localizerCommon["Detail.Edit"]} {_localizer["Supplier"]}|{id}");
        async Task ViewItemAsync(int id)
        {

            _navigation.NavigateTo($"/detailsupplier/{_localizerCommon["Detail.View"]} {_localizer["Supplier"]}|{id}");
        }

        async Task DeleteItemAsync(SupplierTenantDTOEntity model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {model.SupplierName}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                // Tạo thực thể Supplier từ model
                var supplierToDelete = new SupplierEntity
                {
                    Id = model.Id // Sử dụng Id làm khóa chính để xóa
                };

                var res = await _suppliersServices.DeleteAsync(supplierToDelete); // Gọi phương thức xóa với thực thể Supplier

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                    await RefreshDataAsync(); // Cập nhật lại dữ liệu sau khi xóa
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _suppliersServices.GetSupplierWithTenantAsync();
                _dataGrid = null;
                _dataGrid = new List<SupplierTenantDTOEntity>();

                if (!res.Succeeded)
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Get data fail",
                        Duration = 5000
                    });

                _dataGrid = res.Data;

                //await _profileGrid.RefreshDataAsync();

                StateHasChanged();
            }
            catch (UnauthorizedAccessException) { }
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