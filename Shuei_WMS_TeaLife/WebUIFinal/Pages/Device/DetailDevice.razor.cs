using Application.DTOs.Response.Account;
using Application.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
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
        private Status selectStatus;

        List<string> _role = new List<string>() { "User", "Operator" };
        List<GetUserWithRoleResponseDTO>? _users = new List<GetUserWithRoleResponseDTO>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains("Detail")) isDisabled = true;

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Guid.TryParse(sub[1], out Guid guid))
                {
                    Id = guid;
                }
            }

            #region Get info
            if (Id.HasValue && Id != Guid.Empty)
            {
                var arg = await _deviceServices.GetByIdAsync(Id.Value);
                if (arg == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result null",
                        Duration = 1000
                    });

                    return;
                }

            _model.Id = arg.Data.Id;
            _model.Name = arg.Data.Name;
            _model.Type = arg.Data.Type;
            _model.Model = arg.Data.Model;
            _model.ActiveUser = arg.Data.ActiveUser;
            _model.Description = arg.Data.Description;
            _model.OS = arg.Data.OS;
            _model.CPU = arg.Data.CPU;
            _model.Memory = arg.Data.Memory;
            _model.Status = selectStatus.ToString();

                if (!string.IsNullOrEmpty(arg.Data.Status))
                {
                    selectStatus = CommonHelpers.ParseEnum<Status>(arg.Data.Status);
                }
            }
            #endregion
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
        async Task Submit(DeviceEntity arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to Save: {arg.Name}?", "Save", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = selectStatus.ToString();

            if (Title.Contains("Create")) // Add new number sequence
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

                    _navigation.NavigateTo("/device", true);
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

            if (Title.Contains("Edit")) // Update existing number sequence
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

                    _navigation.NavigateTo("/device", true);
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
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete this: {_device.Name}?", "Delete", new ConfirmOptions()
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

                    _navigation.NavigateTo("/device", true);
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
