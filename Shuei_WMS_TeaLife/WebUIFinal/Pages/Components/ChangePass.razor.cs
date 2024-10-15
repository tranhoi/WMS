using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebUIFinal.Pages.Components
{
    public partial class ChangePass
    {
        [Parameter] public string Id { get; set; } = string.Empty;

        private ChangePassRequestDTO _model=new ChangePassRequestDTO();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                _model.Id = GlobalVariable.UserAuthorizationInfo.UserId;
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
        async void Submit(ChangePassRequestDTO arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to change password?", "Change password", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            
            var response = await _authenServices.ChangePassAsync(_model);

            if (!response.Flag)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = response.Message,
                    Duration = 5000
                });

                return;
            }

            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = response.Message,
                Duration = 5000
            });

            _dialogService.Close("Success");
        }
    }
}
