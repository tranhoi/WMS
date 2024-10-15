using Domain.Enums;
using Domain.Entity.authp.Commons;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using ShippingBoxModel = Domain.Entity.WMS.Outbound.ShippingBox;

namespace WebUIFinal.Pages.ShippingBoxs
{
    public partial class DialogCardPageAddNewShippingBox
    {
        [Parameter] public string Title { get; set; }
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

                if (Title.Contains("Detail")) isDisabled = true;

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
            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Guid.TryParse(sub[1], out Guid x))
                {
                    ShippingBoxId = x;
                }
            }

            if (ShippingBoxId.HasValue && ShippingBoxId != Guid.Empty)
            {
                var shippingBox = await _shippingBoxServices.GetByIdAsync((Guid)ShippingBoxId);
                if (shippingBox == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = _localizer["Result shipping box null"],
                        Duration = 1000
                    });

                    return;
                }

                model = shippingBox.Data;
                selectedStatus = shippingBox.Data.Status;
            }
        }

        async void Submit(ShippingBoxModel arg)
        {
            if (Title.Contains("Create"))
            {
                var confirm = await _dialogService.Confirm(_localizer["Do you want to create a new shipping box:"] + $" {arg.BoxName}?", _localizer["Create shipping box"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
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
                        Summary = _localizer["Success"],
                        Detail = _localizer["Successfully created shipping box"],
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingboxlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = _localizer["Failed to create shipping box"],
                        Duration = 5000
                    });
                }
            }

            if (Title.Contains("Edit"))
            {
                var confirm = await _dialogService.Confirm(_localizer["Do you want to update shipping box:"] + $" {arg.BoxName}?", _localizer["Update shipping box"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
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
                        Summary = _localizer["Success"],
                        Detail = _localizer["Successfully edited shipping box"],
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingboxlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = _localizer["Failed to edit shipping box"],
                        Duration = 5000
                    });
                }
            }
            _dialogService.Close(_localizer["Success"]);
        }

        async Task DeleteItemAsync(ShippingBoxModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Are you sure you want to delete shipping box:"] + $" {model.BoxName}?", _localizer["Delete shipping box"], new ConfirmOptions()
                {
                    OkButtonText = _localizer["Yes"],
                    CancelButtonText = _localizer["No"],
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingBoxServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizer["Success"],
                        Detail = _localizer["Delete shipping box"] + $" {model.BoxName} " + _localizer["successfully"],
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingboxlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = _localizer["Failed to delete shipping box"] + $" {model.BoxName}",
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }
    }
}
