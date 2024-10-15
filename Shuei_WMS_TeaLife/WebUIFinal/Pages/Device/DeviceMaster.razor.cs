using Radzen;
using Radzen.Blazor;
using DeviceModel = Domain.Entity.WMS.Device;

namespace WebUIFinal.Pages.Device
{
    public partial class DeviceMaster
    {
        List<DeviceModel>? _dataGrid = null;
        RadzenDataGrid<DeviceModel>? _profileGrid;

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            RefreshDataAsync();
        }

        async Task DeleteItemAsync(DeviceModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]} {_localizer["Device"]}: {model.Name}?", $"{_localizerCommon["Delete"]} {_localizer["Device"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _deviceServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete{model.Name} successfully.",
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

        async Task ViewItemAsync(DeviceModel model)
        {
            _navigation.NavigateTo($"/detaildevice/{_localizerCommon["Detail.View"]} {_localizer["Device"]}|{model.Id}");
        }

        async Task EditItemAsync(DeviceModel model)
        {
            _navigation.NavigateTo($"/detaildevice/{_localizerCommon["Detail.Edit"]} {_localizer["Device"]}|{model.Id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/detaildevice/{_localizerCommon["Detail.Create"]} {_localizer["Device"]}");
        }

        async void RefreshDataAsync()
        {
            try
            {
                var res = await _deviceServices.GetAllAsync();

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
                _dataGrid = new List<DeviceModel>();
                _dataGrid = res.Data.ToList();

                filteredData = _dataGrid;

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
