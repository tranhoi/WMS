using Application.Enums;
using Domain.Entity.Commons;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;
using SuppliersEntity = Domain.Entity.Commons.Supplier;

namespace WebUIFinal.Pages.SupplierPage
{
    public partial class DialogCardPageAddNewSupplier
    {
        [Parameter] public string Title { get; set; }

        private bool isDisabled = false;
        private SuppliersEntity _model = new SuppliersEntity();
        private List<string> _status = new List<string>();
        private Status _selectStatus;

        bool _visibleBtnSubmit = true;
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
                //_selectStatus = Status.Activated;

                if (Title.Contains("|"))
                {
                    if (Title.Contains("View")) _visibleBtnSubmit = false;
                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var res = await _suppliersServices.GetByIdAsync(int.Parse(_id));

                    if (res.Succeeded)
                    {
                        _model = res.Data;
                    }

                    //_selectStatus = Status.Activated.ToString() == _model.Status ? Status.Activated : Status.Inactivated;
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


        async void Submit(SuppliersEntity arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to Save: {arg.SupplierName}?", "Supplier", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = _selectStatus.ToString();

            if (string.IsNullOrEmpty(_id))//Add
            {
                var res = await _suppliersServices.InsertAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully created supplier",
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
                        Detail = "Failed to create supplier",
                        Duration = 5000
                    });
                }
            }

            if (Title.Contains("Edit"))//update
            {
                var res = await _suppliersServices.UpdateAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited supplier",
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
                        Detail = "Failed to edit supplier",
                        Duration = 5000
                    });
                }
            }
        }

        async Task DeleteItemAsync(SuppliersEntity supplier)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete supplier: {supplier.SupplierName}?", "Delete supplier", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _suppliersServices.DeleteAsync(supplier);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete supplier {supplier.SupplierName} successfully.",
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
                        Detail = $"Failed to delete supplier {supplier.SupplierName}.",
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
                    Detail = $"Failed to delete supplier {supplier.SupplierName}.",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
