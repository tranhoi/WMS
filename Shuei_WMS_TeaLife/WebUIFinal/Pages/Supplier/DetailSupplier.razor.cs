using Microsoft.AspNetCore.Components;
using SupplierEntity = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.Supplier
{
    public partial class DetailSupplier
    {
        [Parameter] public string Title { get; set; }
        public string? Id { get; set; }

        private bool _visibleBtnSubmit = true;
        private bool isDisabled = false;
        private SupplierEntity _model = new SupplierEntity();
        private EnumStatus selectStatus;

        List<TenantAuth> tenants = new();

        protected override async Task OnInitializedAsync()
        {
            selectStatus = EnumStatus.Activated;

            if (Title.Contains(_localizerCommon["Detail.Create"])) _visibleBtnSubmit = false;

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
                    var arr = Title.Split('|');
                    Title = arr[0];
                    Id = arr[1];

                    var res = await _suppliersServices.GetByIdAsync(int.Parse(Id));

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

            arg.CompanyId = (int)selectStatus;

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
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
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
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                }
            }
        }

        async Task DeleteItemAsync()
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {_model.SupplierName}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                // Tạo thực thể Supplier từ model
                var supplierToDelete = new SupplierEntity
                {
                    Id = _model.Id // Sử dụng Id làm khóa chính để xóa
                };

                var res = await _suppliersServices.DeleteAsync(supplierToDelete); // Gọi phương thức xóa với thực thể Supplier

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                    await RefreshDataAsync(); // Cập nhật lại dữ liệu sau khi xóa
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
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
            }
        }
    }
}
