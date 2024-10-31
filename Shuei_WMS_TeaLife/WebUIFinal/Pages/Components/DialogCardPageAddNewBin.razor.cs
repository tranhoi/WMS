using Microsoft.AspNetCore.Components;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewBin
    {
        [Parameter] public BinDto _model { get; set; } = new BinDto();
        [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

        bool _visibleBtnSubmit = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (_model.Id == Guid.Empty)
                _visibleBtnSubmit = false;
            await RefreshDataAsync();

            StateHasChanged();
        }

        async Task RefreshDataAsync()
        {
            try
            {
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

        async void Submit(BinDto arg)
        {
            if (_model.Id == Guid.Empty)
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Create"]} {_localizer["Bin"]}: {arg.BinCode}?", $"{_localizer["Create"]} {_localizer["Bin"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }
            else
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Update"]} {_localizer["Bin"]}: {arg.BinCode}?", $"{_localizer["Update"]} {_localizer["Bin"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }

            if (_model.Id == Guid.Empty) _model.Id = Guid.NewGuid();


            _dialogService.Close(_model);
        }

        async Task PrintLable()
        {
            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = "Info",
                Detail = "Print label click",
                Duration = 1000
            });
        }

        async Task AddBin()
        {
            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = "Info",
                Detail = "Add bin click",
                Duration = 1000
            });
        }

        async Task DeleteItemAsync()
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]} {_localizer["Bin"]}: {_model.BinCode}?", $"{_localizer["Delete"]} {_localizer["Bin"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                _model.IsDelete = true;
                _dialogService.Close(_model);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
