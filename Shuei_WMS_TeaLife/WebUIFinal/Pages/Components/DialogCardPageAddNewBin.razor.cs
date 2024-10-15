using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Domain.Enums;
using Application.Extentions;
using Domain.Entity.authp.Commons;
using Domain.Entity.WMS;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Security.Cryptography;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewBin
    {
        [Parameter] public Bin _model { get; set; } = new Bin();
        [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

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

        async void Submit(Bin arg)
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
    }
}
