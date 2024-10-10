using Application.DTOs.Response.Account;
using Domain.Entity.Commons;
using Radzen;
using Radzen.Blazor;
using ShippingCarrierEntity = Domain.Entity.WMS.Outbound.ShippingCarrier;

namespace WebUIFinal.Pages.ShippingCarrier
{
    public partial class ShippingCarrierMaster
    {
        List<ShippingCarrierEntity> _dataGrid = null;
        RadzenDataGrid<ShippingCarrierEntity> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/detailshippingcarrier/Create New Shipping Carrier");

        async Task EditItemAsync(Guid id) => _navigation.NavigateTo($"/detailshippingcarrier/Edit Shipping Carrier|{id}");
        async Task ViewItemAsync(Guid id)
        {

            _navigation.NavigateTo($"/detailshippingcarrier/Detail Shipping Carrier|{id}");
        }

        async Task DeleteItemAsync(ShippingCarrierEntity model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete this: {model.ShippingCarrierName}?", "Delete", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingCarrierServices.DeleteAsync(model);

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
                var res = await _shippingCarrierServices.GetAllAsync();
                _dataGrid = null;
                _dataGrid = new List<ShippingCarrierEntity>();

                if (!res.Succeeded)
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Get data fail",
                        Duration = 5000
                    });

                _dataGrid = res.Data;

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