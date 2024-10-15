using Application.DTOs.Response.Account;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Cryptography;
using WebUIFinal.Core;
using DeviceEntity = Domain.Entity.WMS.Device;

namespace WebUIFinal.Pages.Device
{
    public partial class DetailDevice
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool isDisabled = false;
        private DeviceEntity _model = new DeviceEntity();
        private EnumStatus selectStatus;

        List<string> _role = new List<string>() { "User", "Operator" };
        List<GetUserWithRoleResponseDTO>? _users = new List<GetUserWithRoleResponseDTO>();

        protected override async Task OnInitializedAsync()
        {
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
                    if (Title.Contains(_localizerCommon["Detail.View"])) isDisabled = true;
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
                OkButtonText = "Yes",
                CancelButtonText = "No",
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
                        Summary = "Success",
                        Detail = "Successfully created",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/devicelist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to create",
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

                    _navigation.NavigateTo("/devicelist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to edit",
                        Duration = 5000
                    });
                }
            }
        }
        async Task DeleteItemAsync(DeviceEntity _device)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {_device.Name}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _deviceServices.DeleteAsync(_device);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {_device.Name} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/devicelist", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete {_device.Name}.",
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
                    Detail = $"Failed to delete {_device.Name}.",
                    Duration = 5000
                });
            }
        }
    }
}
