using Application.DTOs.Request.shipment;
using Application.DTOs;


using Radzen.Blazor;
using Radzen;
using WebUIFinal.Core.Dto;
using Microsoft.AspNetCore.Components;
using Application.Models;
using Microsoft.JSInterop;
using System.Net.NetworkInformation;
using Application.DTOs.Response.Product;
using static QRCoder.Core.QRCodeGenerator;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Blazored.LocalStorage;

namespace WebUIFinal.Pages.WarehousePicking
{
    public partial class PickingDetail
    {
        [Inject]
        private ILocalStorageService _localStorage { get; set; }
        WarehousePickingDTO _model { get; set; }
        RadzenDataGrid<WarehousePickingLineDTO> _profileGrid;
        RadzenDataGrid<WarehousePickingShipmentDTO> _profileGridShipment;

        List<WarehousePickingLineDTO> _dataGrid = null;
        List<WarehousePickingShipmentDTO> _dataGridShipment = null;

        string pickNo = null;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool _allowRowSelectOnRowClick = false;
        bool _manualEntry = false;
        string _productScan = string.Empty;

        IList<WarehousePickingLineDTO> _gridSelected = [];

        bool _disableEdit = false;

        private WarehousePickingLineDTO selectedProduct = new WarehousePickingLineDTO() { ProductCode = "default"};
        private List<WarehousePickingLineDTO> pickingDetailsBefore = new List<WarehousePickingLineDTO>();
        private List<WarehousePickingLineDTO> pickingDetailsToUpdate = new List<WarehousePickingLineDTO>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
            await LoadPickingDetailsAsync();
        }

        void ShowTooltip(ElementReference elementReference, TooltipOptions options = null) => _tooltipService.Open(elementReference, $"{_localizer["Manual Entry"]}: {_localizer[_manualEntry.ToString()]}", options);

        async Task OpenAsync(WarehousePickingDTO model)
        {
            var m = model;

            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Info,
                Summary = "Go to packing",
                Detail = $"Shipping No:{model.ShipmentNo}",
                Duration = 5000
            });
        }

        async Task RefreshDataAsync()
        {
            try
            {
                _model = await _localStorage.GetItemAsync<WarehousePickingDTO>("PickingDetail");
                if (_model == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Detail model is null",
                        Duration = 5000
                    });
                    _navigation.NavigateTo("/pickinglist");
                    return;
                }
                pickNo = _model.PickNo;
                if (_model.Status != EnumShipmentOrderStatus.Picking) _disableEdit = true;
                var res = await _warehousePickingLineServices.GetPickingLineDTOAsync(pickNo);
                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                    return;
                }

                _dataGrid = null;
                _dataGrid = new List<WarehousePickingLineDTO>();
                _dataGrid = res.Data.ToList();

                var shipments = await _warehousePickingLineServices.GetShipmentsByPickAsync(pickNo);
                if (!shipments.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });
                    return;
                }

                _dataGridShipment = null;
                _dataGridShipment = new List<WarehousePickingShipmentDTO>();
                _dataGridShipment = shipments.Data.ToList();

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
        async Task CompleteAsync()
        {
            if (pickingDetailsToUpdate.Any())
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = _localizerCommon["Warning"],
                    Detail = "You must keep the pick before completing it.",
                    Duration = 5000
                });
                return;
            }
            string messengerconfirm = $"{_localizerCommon["Confirmation.Complete"]}: {pickNo}";
            foreach (var pickDetail in _dataGrid)
            {
                if (pickDetail.Remaining != 0)
                {
                    messengerconfirm += ", This pick has details that have not been picked in sufficient quantity";
                }
            }
            try
            {
                var confirm = await _dialogService.Confirm(messengerconfirm + "?", _localizerCommon["Complete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _warehousePickingListServices.CompletePickingAsync(pickNo);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _localizerCommon["Success"],
                        Detail = $"{pickNo} completed.",
                        Duration = 5000
                    });
                    await _localStorage.RemoveItemAsync("PickingDetail");
                    await Task.Delay(1000);
                    _navigation.NavigateTo("/pickinglist", true);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = $"{pickNo} completion failed.",
                    Duration = 5000
                });
            }
        }
        async Task HTSync() 
        {
            var result = await _warehousePickingListServices.SyncToHTAsync(_dataGrid);
            _dataGrid = result.Data;
            foreach (var row in _dataGrid)
            {
                pickingDetailsToUpdate.Add(row);
                await _profileGrid.UpdateRow(row);
            }
        }
        async Task DeleteAsync()
        {
            try
            {               
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {pickNo}?" + "The picking lines will be deleted, and the assigned shipments will be unassigned", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _warehousePickingListServices.DeletePickingAsync(pickNo);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {pickNo} successfully.",
                        Duration = 5000
                    });

                    await Task.Delay(1000);
                    _navigation.NavigateTo("/pickinglist", true);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"Failed to delete {pickNo}.",
                    Duration = 5000
                });
            }
        }
        async Task Cancel() {
            pickingDetailsBefore.Clear();
            pickingDetailsToUpdate.Clear();
            _navigation.NavigateTo("/pickinglist");

        }
        async Task EditRow(WarehousePickingLineDTO detail)
        {
            if (!CheckSelectedProduct())
            {
                await _profileGrid.EditRow(detail);
                selectedProduct = detail;
            }
        }
        async Task SaveRow(WarehousePickingLineDTO detail)
        {
            if (string.IsNullOrEmpty(detail.ProductCode))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _localizerCommon["Error"],
                    Detail = _localizer["ProductIsRequired"],
                    Duration = 4000
                });
                return;
            }
            var existingDetail = pickingDetailsToUpdate.FirstOrDefault(d => d.ProductCode == detail.ProductCode);

            if (existingDetail != null)
            {
                int index = pickingDetailsToUpdate.IndexOf(existingDetail);
                pickingDetailsToUpdate[index] = detail;
            }
            else
            {
                pickingDetailsToUpdate.Add(detail);
            }
            selectedProduct = null;
            await _profileGrid.UpdateRow(detail);
        }
        void CancelEdit(WarehousePickingLineDTO detail)
        {
            selectedProduct = null;
            _profileGrid.CancelEditRow(detail);
        }

        private async Task KeepAsync()
        {
            if (!CheckSelectedProduct())
            {
                var changedDetails = pickingDetailsToUpdate.Where(updatedDetail =>
                {
                    var originalDetail = pickingDetailsBefore
                        .FirstOrDefault(beforeDetail => beforeDetail.ProductCode == updatedDetail.ProductCode);
                    return originalDetail == null || !AreDetailsEqual(originalDetail, updatedDetail);
                }).ToList();

                // Check the count of changedDetails
                if (changedDetails.Count <= 0)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Info,
                        Summary = "Notification",
                        Detail = "No changes detected",
                        Duration = 5000
                    });
                    return; // Exit the method if there are no changes
                }

                // Call UpdateWarehousePickingLinesAsync and check the result
                var updateResult = await _warehousePickingLineServices.UpdateWarehousePickingLinesAsync(changedDetails);

                if (updateResult.Succeeded)
                {
                    pickingDetailsToUpdate.Clear();
                    await RefreshDataAsync();
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Update successful",
                        Duration = 5000
                    });
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Failure",
                        Detail = updateResult.Messages.FirstOrDefault() ?? "An error occurred during the update",
                        Duration = 5000
                    });
                }
            }
        }

        bool AreDetailsEqual(WarehousePickingLineDTO detail1, WarehousePickingLineDTO detail2)
        {
            return detail1.ActualQty == detail2.ActualQty;
        }


        private List<EnumDisplay<EnumShipmentOrderStatus>> GetDisplayStatus()
        {
            return Enum.GetValues(typeof(EnumShipmentOrderStatus)).Cast<EnumShipmentOrderStatus>().Select(_ => new EnumDisplay<EnumShipmentOrderStatus>
            {
                Value = _,
                DisplayValue = GetValueLocalizedStatus(_)
            }).ToList();
        }

        private string GetValueLocalizedStatus(EnumShipmentOrderStatus enumStatus)
        {
            return _localizerEnum[enumStatus.ToString()];
        }
        private async Task LoadPickingDetailsAsync()
        {
            var res = await _warehousePickingLineServices.GetPickingLineDTOAsync(pickNo);

            if (!res.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = res.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }
            pickingDetailsBefore = res.Data.ToList();
        }
        private bool CheckSelectedProduct()
        {
            if (selectedProduct != null && selectedProduct.ProductCode != "default")
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Warning",
                    Detail = "You must complete the previous adjustments first",
                    Duration = 5000
                });
                return true;
            }

            return false;
        }
    }
}
