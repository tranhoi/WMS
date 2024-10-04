using Application.DTOs.Response.Account;
using Domain.Entity.Commons;
using Radzen;
using Radzen.Blazor;
using SupplierEntity = Domain.Entity.Commons.Supplier;

namespace WebUIFinal.Pages.SupplierPage
{
    public partial class SupplierList
    {
        List<SupplierEntity> _dataGrid = null;
        RadzenDataGrid<SupplierEntity> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/addsupplier/Create New Supplier");

        async Task EditItemAsync(int id) => _navigation.NavigateTo($"/addsupplier/Edit Supplier|{id}");
        async Task ViewItemAsync(int id)
        {

            _navigation.NavigateTo($"/addsupplier/View Supplier|{id}");
        }

        async Task DeleteItemAsync(SupplierEntity model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete supplier: {model.SupplierName}?", "Delete supplier", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _suppliersServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                    StateHasChanged();
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

                await RefreshDataAsync();
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

                return;
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _suppliersServices.GetAllAsync();
                _dataGrid = null;
                _dataGrid = new List<SupplierEntity>();

                if (!res.Succeeded)
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Get supplier fail",
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