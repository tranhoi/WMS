using Radzen;
using Radzen.Blazor;
using BatchModel = Domain.Entity.WMS.Batches;

namespace WebUIFinal.Pages.Batch
{
    public partial class BatchMaster
    {
        List<BatchModel>? _dataGrid = null;

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            RefreshDataAsync();
        }

        async Task DeleteItemAsync(BatchModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]} {_localizer["Batch"]}: {model.Id}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _batchServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete{model.Id} successfully.",
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

        async Task ViewItemAsync(BatchModel model)
        {
            _navigation.NavigateTo($"/detailbatch/{_localizerCommon["Detail.View"]} {_localizer["Batch"]}|{model.Id}");
        }

        async Task EditItemAsync(BatchModel model)
        {
            _navigation.NavigateTo($"/detailbatch/{_localizerCommon["Detail.Edit"]} {_localizer["Batch"]}|{model.Id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo($"/detailbatch/{_localizerCommon["Detail.Create"]} {_localizer["Batch"]}");
        }

        async void RefreshDataAsync()
        {
            try
            {
                var res = await _batchServices.GetAllAsync();

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
                _dataGrid = new List<BatchModel>();
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
