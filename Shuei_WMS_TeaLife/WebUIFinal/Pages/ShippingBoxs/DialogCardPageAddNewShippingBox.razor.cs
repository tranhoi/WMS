using Microsoft.AspNetCore.Components;
using ShippingBoxModel = FBT.ShareModels.WMS.ShippingBox;

namespace WebUIFinal.Pages.ShippingBoxs
{
    public partial class DialogCardPageAddNewShippingBox
    {
        [Parameter] public string Mode { get; set; }
        public string Title { get; set; }
        public Guid? ShippingBoxId { get; set; }

        private EnumStatus selectedStatus;
        private bool isDisabled = false;
        private string? imageBase64String;
        bool _showPagerSummary = true;
        bool allowRowSelectOnRowClick = true;
        bool _visibleBtnSubmit = true;

        ShippingBoxModel model = new ShippingBoxModel();
        List<TenantAuth> tenants = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                selectedStatus = EnumStatus.Activated;

                await GetShippingBoxDetail();
                await GetTenantsAsync();
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                StateHasChanged();
            }
        }

        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            tenants.AddRange(data.Data);
        }

        private async Task GetShippingBoxDetail()
        {
            if (Mode.StartsWith("Edit"))
            {
                Title = _localizer["EditShippingBox"];
                var sub = Mode.Split('|');
                Mode = sub[0];

                if (Guid.TryParse(sub[1], out Guid x))
                {
                    ShippingBoxId = x;
                }
                if (ShippingBoxId.HasValue && ShippingBoxId != Guid.Empty)
                {
                    var shippingBox = await _shippingBoxServices.GetByIdAsync((Guid)ShippingBoxId);
                    if (shippingBox == null)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _CLoc["Error"],
                            Detail = _localizer["ShippingBoxIsNotExisted"],
                            Duration = 1000
                        });

                        return;
                    }

                    model = shippingBox.Data;
                    selectedStatus = shippingBox.Data.Status;
                }
            }
            else
            {
                Title = _localizer["CreateShippingBox"];
            }


        }

        async void Submit(ShippingBoxModel arg)
        {
            if (Mode.StartsWith("Create"))
            {
                var confirm = await _dialogService.Confirm(_localizer["DoYouWantToCreateANewShippingBox"], _localizer["CreateShippingBox"], new ConfirmOptions()
                {
                    OkButtonText = _CLoc["Yes"],
                    CancelButtonText = _CLoc["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                model.Status = selectedStatus;

                var response = await _shippingBoxServices.InsertAsync(arg);

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _CLoc["Success"],
                        Detail = _localizer["SuccessfullyCreatedShippingBox"],
                        Duration = 5000
                    });

                    //_navigation.NavigateTo("/shippingboxlist", true);
                }
                else
                {
                    string error = "";
                    response.Messages.ForEach(item =>
                    {
                        error += _localizer[item];
                    });
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = _localizer["FailedToCreateShippingBox"] + error,
                        Duration = 5000
                    });
                }
            }

            if (Mode.Contains("Edit"))
            {
                var confirm = await _dialogService.Confirm(_localizer["DoYouWantToUpdateShippingBox"], _localizer["UpdateShippingBox"], new ConfirmOptions()
                {
                    OkButtonText = _CLoc["Yes"],
                    CancelButtonText = _CLoc["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                model.Status = selectedStatus;

                var response = await _shippingBoxServices.UpdateAsync(model);

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _CLoc["Success"],
                        Detail = _localizer["SuccessfullyEditedShippingBox"],
                        Duration = 5000
                    });

                    //_navigation.NavigateTo("/shippingboxlist", true);
                }
                else
                {
                    string error = ":";
                    response.Messages.ForEach(item =>
                    {
                        error += _localizer[item];
                    });
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = _localizer["FailedToEditShippingBox"] + error,
                        Duration = 5000
                    });
                }
            }
            _dialogService.Close(_CLoc["Success"]);
        }

        async Task DeleteItemAsync(ShippingBoxModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{model.BoxName}" + _localizer["AreYouSureYouWantToDeleteShippingBox"], _localizer["DeleteShippingBox"], new ConfirmOptions()
                {
                    OkButtonText = _CLoc["Yes"],
                    CancelButtonText = _CLoc["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingBoxServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _CLoc["Success"],
                        Detail = _localizer["DeleteShippingBox"] + $" {model.BoxName} " + _localizer["successfully"],
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingboxlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _CLoc["Error"],
                        Detail = _localizer["FailedToEditShippingBox"] + $" {model.BoxName}",
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }
    }
}
