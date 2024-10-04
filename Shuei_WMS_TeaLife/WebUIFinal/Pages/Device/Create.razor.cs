using Application.DTOs.Response.Account;
using Application.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using DeviceModel = Domain.Entity.WMS.Device;

namespace WebUIFinal.Pages.Device
{
    public partial class Create
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public DeviceModel _model { get; set; } = new DeviceModel();

        Status _selectStatus;

        List<string> _role = new List<string>() { "User", "Operator" };
        List<GetUserWithRoleResponseDTO>? _users = new List<GetUserWithRoleResponseDTO>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (_model.Id == Guid.Empty)
            {
                _selectStatus = Status.Activated;
            }
            else
                _selectStatus = Status.Activated.ToString() == _model.Status ? Status.Activated : Status.Inactivated;

            if (Title.Contains("edit"))
            {
                var res = await _deviceServices.GetByIdAsync(Guid.Parse(Title.Split('|')[1]));

                if (!res.Succeeded)
                {
                    //notify
                }
                _model = res.Data;
            }
            StateHasChanged();
            GetUsersWithRole();
        }

        async Task GetUsersWithRole()
        {
            try
            {
                var res = await _authenServices.GetUsersWithRolesAsync();
                _users = null;
                _users = new List<GetUserWithRoleResponseDTO>();

                if (res != null)
                    _users.AddRange(res);

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

        async Task Cancel()
        {
            _navigation.NavigateTo("/devicelist");
        }
        async void Submit(DeviceModel arg)
        {
            if (_model.Id == Guid.Empty)
            {
                var confirm = await _dialogService.Confirm($"Do you want to create a new device: {arg.Name}?", "Create", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }
            else
            {
                var confirm = await _dialogService.Confirm($"Do you want to update device: {arg.Name}?", "Update", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }

            _model.Name = arg.Name;
            _model.Type = arg.Type;
            _model.Model = arg.Model;
            _model.ActiveUser = arg.ActiveUser;
            _model.Description = arg.Description;
            _model.OS = arg.OS;
            _model.Memory = arg.Memory;
            _model.Status = _selectStatus.ToString();

            string resMess = string.Empty;
            if (_model.Id == Guid.Empty)
            {
                _model.Id = Guid.NewGuid();
                var response = await _deviceServices.InsertAsync(_model);

                if (!response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = response.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                    return;
                }

                resMess = response.Messages.FirstOrDefault();
            }
            else
            {
                var response = await _deviceServices.UpdateAsync(_model);

                if (!response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = response.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                    return;
                }

                resMess = response.Messages.FirstOrDefault();
            }

            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = resMess,
                Duration = 5000
            });

            _dialogService.Close("Success");
            _navigation.NavigateTo("/devicelist");
        }
    }
}
