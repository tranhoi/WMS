using Application.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;
using VendorEntity = Domain.Entity.Commons.Vendor;

namespace WebUIFinal.Pages.Vendor
{
    public partial class DialogCardPageAddNewVendor
    {
        [Parameter] public string Title { get; set; }
        public int? VendorId { get; set; }

        private bool isDisabled = false;
        private VendorEntity _model = new VendorEntity();
        private List<string> _status = new List<string>();
        private Status selectStatus;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains("Detail")) isDisabled = true;

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Int32.TryParse(sub[1], out int x)) 
                { 
                    VendorId = x;
                }
            }

            #region Get vendor info
            if (VendorId.HasValue && VendorId > 0)
            {
                var resultVendor = await _vendorsServices.GetByIdAsync((int)VendorId);
                if (resultVendor == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result vendor null",
                        Duration = 1000
                    });

                    return;
                }

                _model.Id = resultVendor.Data.Id;
                _model.VendorCode = resultVendor.Data.VendorCode;
                _model.VendorName = resultVendor.Data.VendorName;
                _model.BillingAddress = resultVendor.Data.BillingAddress;
                _model.BillingPhone = resultVendor.Data.BillingPhone;
                _model.BillingMail = resultVendor.Data.BillingMail;
                _model.HeadOfficeFax = resultVendor.Data.HeadOfficeFax;
                _model.BankName = resultVendor.Data.BankName;
                _model.BankBranch = resultVendor.Data.BankBranch;
                _model.BankAccountNumber = resultVendor.Data.BankAccountNumber;
                _model.BankAccountHolder = resultVendor.Data.BankAccountHolder;

                if (!string.IsNullOrEmpty(resultVendor.Data.Status))
                {
                    selectStatus = CommonHelpers.ParseEnum<Status>(resultVendor.Data.Status);
                }
            }
            #endregion

            StateHasChanged();
        }

        async void Submit(VendorEntity arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to Save: {arg.VendorName}?", "vendor", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = selectStatus.ToString();

            if (Title.Contains("Create"))//Add
            {
                var res = await _vendorsServices.InsertAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully created vendor",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/vendorlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to create vendor",
                        Duration = 5000
                    });
                }
            }

            if (Title.Contains("Edit"))//update
            {
                var res = await _vendorsServices.UpdateAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited vendor",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/vendorlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to edit vendor",
                        Duration = 5000
                    });
                }
            }
        }

        async Task DeleteItemAsync(VendorEntity vendor)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete vendor: {vendor.VendorName}?", "Delete vendor", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _vendorsServices.DeleteAsync(vendor);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete vendor {vendor.VendorName} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/vendorlist", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete vendor {vendor.VendorName}.",
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
                    Detail = $"Failed to delete vendor {vendor.VendorName}.",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
