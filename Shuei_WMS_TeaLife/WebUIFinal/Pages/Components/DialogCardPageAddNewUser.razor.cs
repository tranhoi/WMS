using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Domain.Enums;
using Domain.Entity.authp.Commons;
using Domain.Entity.WMS;
using Mapster;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using QRCoder;
using Microsoft.JSInterop;
using QRCoder.Core;
using WebUIFinal.TemplateHtmlPrintLabel;
using WebUIFinal.Pages.Account;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewUser
    {
        [Parameter] public string Title { get; set; }

        string _id = string.Empty;
        CreateAccountRequestDTO _model = new CreateAccountRequestDTO();

        List<CreateRoleRequestDTO> _roles = new List<CreateRoleRequestDTO>();
        IList<string> _selectedRoles = [];

        List<string> _status = new List<string>();
        EnumStatus _selectStatus;

        List<TenantAuth> _tenantList = [];
        IList<TenantAuth> _selectedTenantList = [];
        IList<string> _selectedTenant = [];

        RadzenDataGrid<TenantAuth> _profileGrid;
        bool allowRowSelectOnRowClick = true;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 50, 100 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        bool password = true;
        void TogglePassword()
        {
            password = !password;
        }
        bool _visibleBtnSubmit = true, _disable =false;

        List<UserToTenant> tenantCurrent = new List<UserToTenant>();//danh sach cac tenant duoc dang ky cho user
        List<UserToTenant> tenantNew = new List<UserToTenant>();//danh sach cac tenant duoc dang ky cho user

        private string inputText = string.Empty;
        private string qrCodeBase64 = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                //get tenant
                var resultTenant = await _tenantsServices.GetAllAsync();
                if (resultTenant.Succeeded)
                    _tenantList = resultTenant.Data;

                var result = await _authenServices.GetRolesAsync();

                foreach (var role in result)
                {
                    _roles.Add(new CreateRoleRequestDTO() { Name = role.Name, Id = role.Id });
                }

                if (Title.Contains("|"))
                {
                    var sub = Title.Split('|');
                    Title = sub[0];
                    _id = sub[1];

                    if (string.IsNullOrEmpty(_id))
                    {
                        var user = await _authenServices.UserGetByEmailAsync(GlobalVariable.UserAuthorizationInfo.EmailName);
                        if (user != null) { _id = user.Id; }
                    }

                    #region Get user info
                    var resultUser = await _authenServices.UserGetById(_id);
                    if (resultUser == null)
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

                    _model.Email = resultUser.Email;
                    _model.UserName = resultUser.UserName;
                    _model.FullName = resultUser.FullName;
                    _model.Status = resultUser.Status;
                    foreach (var role in resultUser.Roles)
                    {
                        _model.Roles.Add(new CreateRoleRequestDTO()
                        {
                            Id = role.Id,
                            Name = role.Name,
                        });

                        _selectedRoles.Add(role.Id);
                    }

                    var resultU2T = await _userToTenantServices.GetByUserIdAsync(resultUser.Id);

                    if (!resultU2T.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = "Result user to tenant null",
                            Duration = 1000
                        });

                        return;
                    }

                    tenantCurrent = resultU2T.Data;

                    foreach (var item in tenantCurrent)
                    {
                        var r = _tenantList.FirstOrDefault(x => x.TenantId == item.TenantId);
                        _selectedTenantList.Add(r);
                    }
                    #endregion

                    if (Title.Contains($"{_localizer["Detail.View"]}"))
                    {
                        _visibleBtnSubmit = false;
                        _disable = true;
                    }
                }

                //Title = Title.Contains("View") ? $"{_localizer["Detail.View"]} User" : Title.Contains("Edit") ? $"{_localizer["Detail.Edit"]} User" : $"{_localizer["Detail.Create"]} User";

                _selectStatus = EnumStatus.Activated;

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

        async void Submit(CreateAccountRequestDTO arg)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Create"]} {_localizer["User"]}: {arg.UserName}?", $"{_localizer["Create"]} {_localizer["User"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                //If edit user then clear roles for adding new roles
                if (!string.IsNullOrEmpty(_id))
                {
                    arg.Roles = null;
                    arg.Roles = new List<CreateRoleRequestDTO>();

                    //get danh sach tenant duoc dang ky cho user
                    var resU2T = await _userToTenantServices.GetByUserIdAsync(_id);
                }

                //lay dang sachs role moi
                foreach (var role in _selectedRoles)
                {
                    var r = _roles.FirstOrDefault(x => x.Id == role);
                    arg.Roles.Add(new CreateRoleRequestDTO() { Id = r.Id, Name = r.Name });
                }

                arg.Status = _selectStatus;
                arg.ConfirmPassword = arg.Password;

                //lay danh sach tenant moi dc chon
                foreach (var item in _selectedTenantList)
                {
                    var r = _tenantList.FirstOrDefault(x => x.TenantId == item.TenantId);
                    tenantNew.Add(new UserToTenant()
                    {
                        Id = Guid.NewGuid(),
                        UserId = _id,
                        TenantId = item.TenantId
                    });
                }

                if (Title.Contains(_localizer["Detail.Create"]))//Add
                {
                    var res = await _authenServices.CreateAccountAsync(arg);

                    var user = await _authenServices.UserGetByEmailAsync(arg.Email);

                    foreach (var item in tenantNew)
                    {
                        item.UserId = user.Id;
                    }

                    var resU2T = await _userToTenantServices.AddRangeAsync(tenantNew);
                    if (res.Flag && resU2T.Succeeded)
                    {

                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = res.Message,
                            Duration = 5000
                        });

                        //_navigation.NavigateTo("/userlist");
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = res.Message,
                            Duration = 5000
                        });
                    }
                }
                else if (Title.Contains(_localizer["Detail.Edit"]))//update
                {
                    var userInfoUpdate = new UpdateUserInfoRequestDTO();
                    userInfoUpdate.Id = _id;
                    userInfoUpdate.UserName = arg.UserName;
                    userInfoUpdate.Email = arg.Email;
                    userInfoUpdate.FullName = arg.FullName;
                    userInfoUpdate.Status = arg.Status;
                    userInfoUpdate.Roles = arg.Roles;

                    var res = await _authenServices.UpdateUserInfoAsync(userInfoUpdate);

                    if (res.Flag)
                    {
                        var resU2T = await _userToTenantServices.DeleteRangeAsync(tenantCurrent);
                        resU2T = await _userToTenantServices.AddRangeAsync(tenantNew);

                        if (resU2T.Succeeded)
                            _notificationService.Notify(new NotificationMessage()
                            {
                                Severity = NotificationSeverity.Success,
                                Summary = "Success",
                                Detail = $"{res.Message}|{resU2T.Messages.FirstOrDefault()}",
                                Duration = 5000
                            });

                        //_navigation.NavigateTo("/userlist");
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = $"{res.Message}",
                            Duration = 5000
                        });
                    }
                }

                #region assigned tenant to user

                #endregion
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


        async void RefreshData()
        {
            try
            {
                //_ovenId = int.TryParse(OvenId, out int value) ? value : 0;

                //var res = await _ft01Client.GetAllAsync();

                //if (res == null)
                //    return;
                //_ft01 = res.Data.ToList();

                //_ovenInfo = JsonConvert.DeserializeObject<OvensInfo>(_ft01.FirstOrDefault().C001).FirstOrDefault(x => x.Id == _ovenId);
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

            StateHasChanged();
        }

        async Task PrintLable()
        {
            try
            {
                var dataPrint = await _authenServices.GetLabelByIdAsync(_id);

                var res = await _dialogService.OpenAsync<PrintViewer>(string.Empty,
                        new Dictionary<string, object>() { { "LabelPrintModel", dataPrint }, { "Title", $"Print label for use" } },
                        new DialogOptions()
                        {
                            Width = "1000px",
                            Height = "1000px",
                            Resizable = true,
                            Draggable = true,
                            ShowClose = false,
                            CloseDialogOnOverlayClick = true
                        });
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

        async Task PrintLable1()
        {
            var dataPrint = await _authenServices.GetReportBase64(_id);
            var res = await _dialogService.OpenAsync<ReportViewer>($"Print label for use",
                  new Dictionary<string, object>() { { "_pdfBase64", dataPrint } },
                  new DialogOptions()
                  {
                      Width = "1000px",
                      Height = "1000px",
                      Resizable = true,
                      Draggable = true,
                      ShowClose = false,
                      CloseDialogOnOverlayClick = true
                  });
        }

        async Task DisableUser()
        {
            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = "Info",
                Detail = "Disable click",
                Duration = 1000
            });
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null)
        {
            _tooltipService.Open(elementReference, "Full name", options);
        }

        // Method to generate QR code
        private void GenerateQRCode()
        {
            inputText = "NGUYEN DINH CONG|COng123@456";
            if (string.IsNullOrEmpty(inputText))
                return;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }

        // Method to print the QR code
        private async Task PrintQRCode()
        {
            await _jsRuntime.InvokeVoidAsync("printQRCode");
        }
    }
}
