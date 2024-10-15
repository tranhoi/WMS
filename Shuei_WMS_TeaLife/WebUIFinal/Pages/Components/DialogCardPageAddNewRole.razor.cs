using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewRole
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public CreateRoleRequestDTO _model { get; set; } = new CreateRoleRequestDTO();

        bool _visibleBtnSubmit = true;
        string _id = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if (Title.Contains("|"))
                {
                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    if (Title.Contains($"{_localizer["Detail.View"]}")) _visibleBtnSubmit = false;//View thi an nut Save di

                    var roleResult = await _authenServices.RoleGetById(_id);

                    if (roleResult != null)
                    {
                        _model.Id = roleResult.Id;
                        _model.Name = roleResult.Name;
                    }
                }

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

        async void Submit(CreateRoleRequestDTO arg)
        {
            var response = new GeneralResponse();

            if (Title.Contains(_localizer["Detail.Edit"]))
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Update"] + _localizer["Role"] +  $": { arg.Name} ", _localizer["Update"] + " " + _localizer["Role"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                _model.Name = arg.Name;

                response = await _authenServices.UpdateRoleAsync(new UpdateDeleteRequestDTO() { Id = _model.Id, Name = _model.Name });
            }
            else if (Title.Contains(_localizer["Detail.Create"]))
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Create"] + _localizer["Role"] + $": {arg.Name}?", _localizer["Create"] + " " + _localizer["Role"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                //_model.Id = Guid.NewGuid().ToString();
                _model.Name = arg.Name;

                response = await _authenServices.CreateRoleAsysnc(_model);
            }

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
