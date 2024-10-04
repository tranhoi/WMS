using Radzen;
using Radzen.Blazor;
using NumberSequenceEntity = Domain.Entity.WMS.NumberSequences;

namespace WebUIFinal.Pages.NumberSequence
{
    public partial class NumberSequence
    {
        private List<NumberSequenceEntity> _numberSequences = new List<NumberSequenceEntity>();
        RadzenDataGrid<NumberSequenceEntity> _profileNumberSequenceGrid;
        bool _showPagerSummary = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                var result = await _numberSequenceServices.GetAllAsync();

                if (result != null)
                {
                    _numberSequences.AddRange(result.Data);
                    _filteredModel = _numberSequences;
                }

                _filteredModel = new List<NumberSequenceEntity>(_numberSequences);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
            finally
            {
                await RefreshDataAsync();
            }
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/addnumbersequence/Create Number Sequence");

        async Task EditItemAsync(Guid ID) => _navigation.NavigateTo($"/addnumbersequence/Edit Number Sequence|{ID}");

        async Task DeleteItemAsync(NumberSequenceEntity numberSequence)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete number sequence: {numberSequence.JournalType}?", "Delete", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _numberSequenceServices.DeleteAsync(numberSequence);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Messages.ToString(),
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
                        Detail = res.Messages.ToString(),
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

        void NavigateDetailPage(Guid ID) => _navigation.NavigateTo($"/addnumbersequence/Number Sequence Detail|{ID}");

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _numberSequenceServices.GetAllAsync();
                _numberSequences = null;
                _numberSequences = new();

                if (res != null)
                    _numberSequences.AddRange(res.Data);

                //await _profileGrid.RefreshDataAsync();

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