using Application.DTOs;
using Domain.Entity.WMS;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class DialogCardPageAddNewReceiptLine
    {
        [Parameter] public WarehouseReceiptOrderLineDto warehouseReceiptOrderLine { get; set; } = new ();
        [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

        List<Bin> bins = new List<Bin>();
        List<Unit> units = new List<Unit>();

        private EnumStatus selectedStatus;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await GetBinsAsync();
            await GetBinsAsync();
            await RefreshDataAsync();

            StateHasChanged();
        }

        private async Task GetBinsAsync()
        {
            var data = await _binServices.GetAllAsync();
            bins.AddRange(data.Data);
        }

        private async Task GetUnitsAsync()
        {
            var data = await _unitsService.GetAllAsync();
            units.AddRange(data.Data);
        }

        async Task RefreshDataAsync()
        {
            try
            {
                    selectedStatus = warehouseReceiptOrderLine.Status;

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

        async void Submit(WarehouseReceiptOrderLineDto arg)
        {
            arg.Status = selectedStatus;

            if (warehouseReceiptOrderLine.Id == Guid.Empty)
            {
                var confirm = await _dialogService.Confirm($"Do you want to create a new Receipt Order Line ?", "Create Receipt Order Line", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }
            else
            {
                var confirm = await _dialogService.Confirm($"Do you want to update Receipt Order Line ?", "Update Receipt Order Line", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }

            _dialogService.Close(warehouseReceiptOrderLine);
        }
    }
}
