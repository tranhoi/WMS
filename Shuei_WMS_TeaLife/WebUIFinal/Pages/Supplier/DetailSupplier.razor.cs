using Application.DTOs.Response.Account;
using Domain.Enums;
using Domain.Entity.authp.Commons;
using Microsoft.AspNetCore.Components;
using Radzen;
using System.Security.Cryptography;
using WebUIFinal.Core;
using SupplierEntity = Domain.Entity.Commons.Supplier;

namespace WebUIFinal.Pages.Supplier
{
    public partial class DetailSupplier
    {
        [Parameter] public string Title { get; set; }
        public string? Id { get; set; }

        private bool isDisabled = false;
        private SupplierEntity _model = new SupplierEntity();
        private int? selectStatus;

        List<TenantAuth> tenants = new();

        protected override async Task OnInitializedAsync()
        {
            await RefreshDataAsync();
            await GetTenantsAsync();
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
                    Id = arr[1];

                    var res = await _suppliersServices.GetByIdAsync(int.Parse(Id));

                    if (res.Succeeded)
                    {
                        _model = res.Data;
                        selectStatus = _model.TenantId;
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
        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            tenants.AddRange(data.Data);
        }
        async Task Submit(SupplierEntity arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.SupplierName}?", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.TenantId = (int)selectStatus;

            if (Title.Contains(_localizerCommon["Detail.Create"])) // Add new number sequence
            {
                var res = await _suppliersServices.InsertAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully created",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/supplierlist", true);
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
            else if (Title.Contains(_localizerCommon["Detail.Edit"]))// Update existing number sequence
            {
                var res = await _suppliersServices.UpdateAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully edited",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/supplierlist", true);
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
        async Task DeleteItemAsync(SupplierEntity _supplier)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {_supplier.SupplierName}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _suppliersServices.DeleteAsync(_supplier);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {_supplier.SupplierName} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/supplierlist", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete {_supplier.SupplierName}.",
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
                    Detail = $"Failed to delete {_supplier.SupplierName}.",
                    Duration = 5000
                });
            }
        }
    }
}
