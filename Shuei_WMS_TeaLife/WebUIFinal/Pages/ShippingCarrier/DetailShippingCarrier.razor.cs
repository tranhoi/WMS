using Application.DTOs.Response.Account;
using Application.Enums;
using Domain.Entity.authp.Commons;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Cryptography;
using WebUIFinal.Core;
using ShippingCarrierEntity = Domain.Entity.WMS.Outbound.ShippingCarrier;

namespace WebUIFinal.Pages.ShippingCarrier
{
    public partial class DetailShippingCarrier
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }
        private bool isDisabled = false;
        private ShippingCarrierEntity _model = new ShippingCarrierEntity();

        protected override async Task OnInitializedAsync()
        {
            await RefreshDataAsync();
            await base.OnInitializedAsync();
        }
        async Task RefreshDataAsync()
        {
            try
            {
                if (Title.Contains("|"))
                {
                    if (Title.Contains("Detail")) isDisabled = true;
                    var arr = Title.Split('|');
                    Title = arr[0];
                    Id = Guid.Parse(arr[1]);

                    var res = await _shippingCarrierServices.GetByIdAsync(Id.Value);

                    if (res.Succeeded)
                    {
                        _model = res.Data;
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
            var confirm = await _dialogService.Confirm($"Do you want to save: {arg.ShippingCarrierName}?", "Save", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            if (Title.Contains("Create"))
            {
                var res = await _shippingCarrierServices.InsertAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully created",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingcarrierlist", true);
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

            if (Title.Contains("Edit"))
            {
                var res = await _shippingCarrierServices.UpdateAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully edited",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingcarrierlist", true);
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
        async Task DeleteItemAsync(ShippingCarrierEntity _shippingcarrier)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete this: {_shippingcarrier.ShippingCarrierName}?", "Delete", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _shippingCarrierServices.DeleteAsync(_shippingcarrier);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {_shippingcarrier.ShippingCarrierName} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/shippingcarrierlist", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete {_shippingcarrier.ShippingCarrierName}.",
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
                    Detail = $"Failed to delete {_shippingcarrier.ShippingCarrierName}.",
                    Duration = 5000
                });
            }
        }
    }
}
