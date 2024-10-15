using Radzen;
using Radzen.Blazor;
using ShippingCarrierEntity = Domain.Entity.WMS.Outbound.ShippingCarrier;

namespace WebUIFinal.Pages.ShippingCarrier
{
    public partial class ShippingCarrierMaster
    {
        List<ShippingCarrierEntity>? _dataGrid = null;
        RadzenDataGrid<ShippingCarrierEntity>? _profileGrid;

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            RefreshDataAsync();
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
                        Detail = $"Delete {model.ShippingCarrierName} successfully.",
                        Duration = 5000
                    });

                    RefreshDataAsync();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.ToString(),
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
                return;
            }
        }

        async Task ViewItemAsync(ShippingCarrierEntity model)
        {
            _navigation.NavigateTo($"/detailshippingcarrier/Detail Shipping Carrier|{model.Id}");
        }

        async Task EditItemAsync(ShippingCarrierEntity model)
        {
            _navigation.NavigateTo($"/detailshippingcarrier/Edit Shipping Carrier|{model.Id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo("/detailshippingcarrier/Create Shipping Carrier");
        }

        async void RefreshDataAsync()
        {
            try
            {
                var res = await _shippingCarrierServices.GetAllAsync();

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

                _dataGrid = null;
                _dataGrid = new List<ShippingCarrierEntity>();
                _dataGrid = res.Data.ToList();

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
