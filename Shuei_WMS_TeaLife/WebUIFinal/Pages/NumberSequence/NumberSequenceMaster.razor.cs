using Radzen;
using Radzen.Blazor;
using NumberSequenceModel = Domain.Entity.WMS.NumberSequences;

namespace WebUIFinal.Pages.NumberSequence
{
    public partial class NumberSequenceMaster
    {
        List<NumberSequenceModel>? _dataGrid = null;
        RadzenDataGrid<NumberSequenceModel>? _profileGrid;

        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            RefreshDataAsync();
        }

        async Task DeleteItemAsync(NumberSequenceModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete this: {model.JournalType}?", "Delete", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _numberSequenceServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {model.JournalType} successfully.",
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

        async Task ViewItemAsync(NumberSequenceModel model)
        {
            _navigation.NavigateTo($"/detailnumbersequence/Detail Number Sequence|{model.Id}");
        }

        async Task EditItemAsync(NumberSequenceModel model)
        {
            _navigation.NavigateTo($"/detailnumbersequence/Edit Number Sequence|{model.Id}");
        }

        async Task AddNewItemAsync()
        {
            _navigation.NavigateTo("/detailnumbersequence/Create Number Sequence");
        }

        async void RefreshDataAsync()
        {
            try
            {
                var res = await _numberSequenceServices.GetAllAsync();

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
                _dataGrid = new List<NumberSequenceModel>();
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
