using Application.DTOs.Response.Account;

using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Cryptography;
using WebUIFinal.Core;
using DeviceEntity = FBT.ShareModels.WMS.Device;

namespace WebUIFinal.Pages.Device
{
    public partial class DetailDevice
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        bool _visibleBtnSubmit = true;
        private bool isDisabled = false;
        private DeviceEntity _model = new DeviceEntity();
        private EnumStatus selectStatus;

        List<string> _role = new List<string>() { "User", "Operator" };
        List<GetUserWithRoleResponseDTO>? _users = new List<GetUserWithRoleResponseDTO>();

        protected override async Task OnInitializedAsync()
        {
            selectStatus = EnumStatus.Activated;

            if (Title.Contains(_localizerCommon["Detail.Create"])) _visibleBtnSubmit = false;

            await RefreshDataAsync();
            await GetUsersWithRole();
            await base.OnInitializedAsync();
        }
        async Task RefreshDataAsync()
        {
            try
            {
                if (Title.Contains("|"))
                {
                    var arr = Title.Split('|');
                    Title = arr[0];
                    Id = Guid.Parse(arr[1]);

                    var res = await _deviceServices.GetByIdAsync(Id.Value);

                    if (res.Succeeded)
                    {
                        _model = res.Data;

                        selectStatus=_model.Status;
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
        async Task Submit(DeviceEntity arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.Name}?", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = selectStatus;

            if (Title.Contains(_localizerCommon["Detail.Create"])) // Add new number sequence
            {
                var res = await _deviceServices.InsertAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = _localizerCommon["Success"] + _localizerCommon["Create"],
                        Duration = 5000
                    });
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"])) // Update existing number sequence
            {
                var res = await _deviceServices.UpdateAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully edited",
                        Duration = 5000
                    });
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
        }
        async Task DeleteItemAsync(DeviceEntity model)
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
    }
}
