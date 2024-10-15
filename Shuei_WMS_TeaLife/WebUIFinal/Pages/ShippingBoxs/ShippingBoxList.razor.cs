using Application.DTOs.Response.ShippingBoxs;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using ShippingBoxModel = Domain.Entity.WMS.Outbound.ShippingBox;

namespace WebUIFinal.Pages.ShippingBoxs
{
    public partial class ShippingBoxList
    {
        List<ShippingBoxModel> _shippingBoxes = new();
        RadzenDataGrid<ShippingBoxModel> _profileGrid;
        bool _showPagerSummary = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await RefreshDataAsync();
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

                    await RefreshDataAsync();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
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
                    Summary = _localizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        void EditItemAsync(Guid shippingBoxId) => _navigation.NavigateTo($"/addshippingbox/{_localizer["Edit Shipping Box"]}|" + shippingBoxId);

        void AddNewItemAsync() => _navigation.NavigateTo($"/addshippingbox/{_localizer["Create Shipping Box"]}");

        void NavigateDetailPage(Guid shippingBoxId) => _navigation.NavigateTo($"/addshippingbox/{_localizer["Shipping Box Detail"]}|{shippingBoxId}");

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _shippingBoxServices.GetAllAsync();

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _localizer["Error"],
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }

                _shippingBoxes = res.Data.ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
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
