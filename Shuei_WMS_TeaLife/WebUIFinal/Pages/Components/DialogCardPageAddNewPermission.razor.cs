using Microsoft.AspNetCore.Components;
using Application.DTOs.Response.Account;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewPermission
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public PermissionsListResponseDTO _model { get; set; } = new PermissionsListResponseDTO();

        List<GetRoleResponseDTO> _roles = new List<GetRoleResponseDTO>();
        IList<string> _selectedRoles = [];
        bool _visibleBtnSubmit = true, _disable = false;
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
                if (Title.Contains($"{_localizer["Detail.Create"]}"))
                {
                    _visibleBtnSubmit = false;                    
                }

                if (Title.Contains("|"))
                {
                    //if (Title.Contains($"{_localizer["Detail.Create"]}"))
                    //{
                    //    _visibleBtnSubmit = false;
                    //    _disable = true;
                    //}

                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var resultPermision = await _permissionsServices.GetByIdAsync(Guid.Parse(_id));
                    if ((resultPermision.Succeeded))
                    {
                        var res = resultPermision.Data;
                        _model.Id = res.Id;
                        _model.Name = res.Name;
                        _model.Description = res.Description;
                        _model.CreateAt = res.CreateAt;
                        _model.CreateOperatorId = res.CreateOperatorId;
                    }

                    var r2p = await _roleToPermissionServices.GetByPermissionsIdAsync(_model.Id.ToString());

                    if (r2p.Succeeded)
                    {
                        foreach (var item in r2p.Data)
                        {
                            _selectedRoles.Add(item.RoleId.ToString());

                            _model.AssignedToRoles.Add(new GetRoleResponseDTO()
                            {
                                Id = item.RoleId.ToString(),
                                Name = item.RoleName,
                            });
                        }
                    }
                }

                #region Get role information
                _roles = new List<GetRoleResponseDTO>();

                var result = await _authenServices.GetRolesAsync();

                foreach (var role in result)
                {
                    _roles.Add(role);
                }

                if (_roles == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result user null",
                        Duration = 1000
                    });

                    return;
                }
                #endregion

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
        async void Submit(PermissionsListResponseDTO arg)
        {
            var confirm = await _dialogService.Confirm(_localizer["Confirmation.Create"] + _localizer["Permission.Name"] + $": {arg.Name}?", _localizer["Create"] + " " + _localizer["Permission.Name"], new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            //If edit user then clear roles for adding new roles
            if (_model != null)
            {
                arg.AssignedToRoles = null;
                arg.AssignedToRoles = new List<GetRoleResponseDTO>();
            }

            foreach (var role in _selectedRoles)
            {
                var r = _roles.FirstOrDefault(x => x.Id == role);
                arg.AssignedToRoles.Add(new GetRoleResponseDTO() { Id = r.Id, Name = r.Name });
            }

            _model.Name = arg.Name;
            _model.Description = arg.Description;
            _model.AssignedToRoles = arg.AssignedToRoles;

            var response = await _permissionsServices.AddOrEditAsync(_model);

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

            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = response.Messages.FirstOrDefault(),
                Duration = 5000
            });

            _dialogService.Close("Success");
        }

        async Task DeleteItemAsync(PermissionsListResponseDTO model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Delete"] + _localizer["Permission.Name"] + $": {model.Name}?", _localizer["Delete"] + " " + _localizer["Permission.Name"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _permissionsServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete permission {model.Name} successfully.",
                        Duration = 5000
                    });
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
