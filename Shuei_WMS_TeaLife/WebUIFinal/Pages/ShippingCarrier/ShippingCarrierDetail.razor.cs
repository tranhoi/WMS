using Microsoft.AspNetCore.Components;
using ShippingCarrierEntity = FBT.ShareModels.WMS.ShippingCarrier;

namespace WebUIFinal.Pages.ShippingCarrier
{
    public partial class ShippingCarrierDetail
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool isDisabled = false;
        private EnumStatus selectStatus;
        private ShippingCarrierEntity _model = new ShippingCarrierEntity();

        // Enum values for warehouse transaction types
        private List<dynamic> warehouseTransTypes = new List<dynamic>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            selectStatus = EnumStatus.Activated;

            await RefreshDataAsync();
            StateHasChanged();
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

                    var res = await _shippingCarrierServices.GetByIdAsync(Id.Value);

                    if (res.Succeeded)
                    {
                        _model = res.Data;
                        selectStatus = _model.Status;
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
        async Task Submit(ShippingCarrierEntity arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.ShippingCarrierName}?", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = _localizerCommon["Yes"],
                CancelButtonText = _localizerCommon["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = selectStatus;

            if (Title.Contains(_localizerCommon["Detail.Create"]))
            {
                var res = await _shippingCarrierServices.InsertAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = _localizerCommon["Success"] + _localizerCommon["Create"],
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingcarrierlist", true);
                }
                else
                {
                    string error = ":";
                    res.Messages.ForEach(item =>
                    {
                        error += _localizer[item];
                    });
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to create" + error,
                        Duration = 5000
                    });
                }
            }

            if (Title.Contains(_localizerCommon["Detail.Edit"]))//update
            {
                var res = await _shippingCarrierServices.UpdateAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited shipping carrier",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingcarrierlist", true);
                }
                else
                {
                    string error = ":";
                    res.Messages.ForEach(item =>
                    {
                        error += _localizer[item];
                    });
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to edit shipping carrier" + error,
                        Duration = 5000
                    });
                }
            }
        }

        async Task DeleteItemAsync(ShippingCarrierEntity shippingCarrier)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete shipping carrier: {shippingCarrier.ShippingCarrierName}?", "Delete shipping carrier", new ConfirmOptions()
                {
                    OkButtonText = _localizerCommon["Yes"],
                    CancelButtonText = _localizerCommon["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingCarrierServices.DeleteAsync(shippingCarrier);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete shipping carrier {shippingCarrier.ShippingCarrierName} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingcarrierlistlist", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete shipping carrier {shippingCarrier.ShippingCarrierName}.",
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
                    Detail = $"Failed to delete shipping carrier {shippingCarrier.ShippingCarrierName}.",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
