using Application.DTOs;
using Application.Extentions;
using Domain.Entity.authp.Commons;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Reflection.Metadata;
using WebUIFinal.Core.Dto;
using SupplierModel = Domain.Entity.Commons.Supplier;
using WarehouseReceiptOrderLine = Domain.Entity.WMS.Inbound.WarehouseReceiptOrderLine;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class DialogCardPageAddNewReceipt
    {
        [Parameter] public string Title { get; set; }
        public string? ReceiptNo { get; set; }

        private bool isDisabled = false;
        private bool isDisabledStatus = false;
        private bool _showPagerSummary = true;
        private bool allowRowSelectOnRowClick = true;
        private bool _visibleBtnSubmit = true;
        private EnumReceiptStatus? selectedReceiptStatus;

        private WarehouseReceiptOrderDto warehouseReceiptOrder = new();
        private List<TenantAuth> tenants = new();
        private List<LocationDisplayDto> locations = new();
        private List<WarehouseReceiptOrderLineDto> warehouseReceiptOrderLines = [];
        private List<SupplierModel> suppliers = new();
        private RadzenDataGrid<WarehouseReceiptOrderLineDto>? _profileGrid;
        private IList<WarehouseReceiptOrderLineDto> selectedReceiptOrderLines = [];

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();
                SetInitialState();
                await Task.WhenAll(
                    GetReceiptOrderAsync(),
                    GetTenantsAsync(),
                    GetLocationssAsync(),
                    GetSupplierAsync()
                );
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, "Error", e.Message);
            }
        }

        private void SetInitialState()
        {
            if (Title.Contains("Detail"))
            {
                isDisabled = true;
                isDisabledStatus = true;
                _visibleBtnSubmit = false;
            }

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];
                ReceiptNo = sub[1];
            }
        }

        private async Task GetReceiptOrderAsync()
        {
            if (string.IsNullOrEmpty(ReceiptNo)) return;

            var data = await _warehouseReceiptOrderService.GetReceiptOrderAsync(ReceiptNo);

            if (!data.Succeeded)
            {
                ShowNotification(NotificationSeverity.Error, "Get receipt order error", data.Messages.FirstOrDefault());
                return;
            }

            warehouseReceiptOrder = data.Data;
            selectedReceiptStatus = warehouseReceiptOrder.Status;
            warehouseReceiptOrderLines.AddRange(data.Data.WarehouseReceiptOrderLines);

            await RefreshGrid();

            if (warehouseReceiptOrder.Status == EnumReceiptStatus.Close)
            {
                isDisabled = true;
                isDisabledStatus = true;
                _visibleBtnSubmit = false;
            }
        }

        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            if (data.Succeeded) tenants.AddRange(data.Data);
        }

        private async Task GetLocationssAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        private async Task GetSupplierAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            if (data.Succeeded) suppliers.AddRange(data.Data);
        }

        async Task Submit(WarehouseReceiptOrderDto arg)
        {
            arg.Status = selectedReceiptStatus;
            warehouseReceiptOrder.Status = selectedReceiptStatus;
            arg.WarehouseReceiptOrderLines = warehouseReceiptOrderLines;

            if (Title.Contains("Create"))
            {
                await CreateReceipt(arg);
            }
            else if (Title.Contains("Edit"))
            {
                await UpdateReceipt(arg);
            }
        }

        private async Task CreateReceipt(WarehouseReceiptOrderDto arg)
        {
            if (!await ConfirmAction("Do you want to create a new receipt ?", "Create Receipt")) return;

            warehouseReceiptOrder.ReceiptNo = string.Empty;
            var response = await _warehouseReceiptOrderService.InsertWarehouseReceiptOrder(arg);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, "Success", "Successfully created receipt");
                _navigation.NavigateTo("/warehouse-receiptlist", true);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Failed to create receipt");
            }
        }

        private async Task UpdateReceipt(WarehouseReceiptOrderDto arg)
        {
            if (!await ConfirmAction("Do you want to update ?", "Update Receipt")) return;

            var response = await _warehouseReceiptOrderService.UpdateWarehouseReceiptOrder(arg);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, "Success", "Successfully edited receipt");
                _navigation.NavigateTo("/warehouse-receiptlist", true);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Failed to edit receipt");
            }
        }

        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(message, title, new ConfirmOptions
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true
            }) ?? false;
        }

        private void ShowNotification(NotificationSeverity severity, string summary, string detail, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = severity,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        private Task RefreshGrid() => _profileGrid?.RefreshDataAsync() ?? Task.CompletedTask;

        private async Task AddReceiptOrderLine()
        {
            try
            {
                var warehouseReceiptOrderLineInfo = new WarehouseReceiptOrderLineDto
                {
                    ReceiptNo = warehouseReceiptOrder.ReceiptNo,
                };

                var res = await _dialogService.OpenAsync<DialogCardPageAddNewReceiptLine>("Create new Receipt Order Line", 
                    new Dictionary<string, object> { { "warehouseReceiptOrderLine", warehouseReceiptOrderLineInfo }, { "VisibleBtnSubmit", true } },
                    new DialogOptions { Width = "800", Height = "400", Resizable = true, Draggable = true, CloseDialogOnOverlayClick = true });

                if (res is WarehouseReceiptOrderLineDto selectResult)
                {
                    if (IsDuplicateReceiptOrderLine(selectResult))
                    {
                        ShowNotification(NotificationSeverity.Error, "Error", $"{selectResult.Id} already exists.");
                        return;
                    }

                    await UpdateReceiptOrderLineWithProductDetails(selectResult);
                    warehouseReceiptOrderLines.Add(selectResult);
                    await RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, "Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private bool IsDuplicateReceiptOrderLine(WarehouseReceiptOrderLineDto selectResult)
        {
            var existingLine = warehouseReceiptOrderLines.FirstOrDefault(x => x.Id == selectResult.Id);
            return existingLine?.ProductCode == selectResult.ProductCode && existingLine?.LotNo == selectResult.LotNo;
        }

        private async Task UpdateReceiptOrderLineWithProductDetails(WarehouseReceiptOrderLineDto selectResult)
        {
            var product = await _productServices.GetByProductCodeAsync(selectResult.ProductCode);
            if (product?.Data != null)
            {
                selectResult.UnitId = product.Data.UnitId;
                selectResult.UnitName = product.Data.UnitName;
                selectResult.ProductName = product.Data.ProductName;
                selectResult.StockAvailableQuantity = product.Data.StockAvailableQuantity;
                selectResult.ReceiptNo = string.Empty;
            }
        }

        private async Task ViewReceiptOrderLineItemAsync(WarehouseReceiptOrderLineDto dto)
        {
            var res = await _dialogService.OpenAsync<DialogCardPageAddNewReceiptLine>("View Receipt Order Line",
                new Dictionary<string, object> { { "warehouseReceiptOrderLine", dto }, { "VisibleBtnSubmit", false } },
                new DialogOptions { Width = "800", Height = "400", Resizable = true, Draggable = true, CloseDialogOnOverlayClick = true });

            if (res == "Success")
            {
                await RefreshDataAsync();
            }
        }

        private async Task EditReceiptOrderLineItemAsync(WarehouseReceiptOrderLineDto model)
        {
            await _dialogService.OpenAsync<DialogCardPageAddNewReceiptLine>("Edit Receipt Order Line",
                new Dictionary<string, object> { { "warehouseReceiptOrderLine", model }, { "VisibleBtnSubmit", true } },
                new DialogOptions { Width = "1000", Height = "400", Resizable = true, Draggable = true, CloseDialogOnOverlayClick = true });

            await RefreshGrid();
        }

        private async Task DeleteReceiptOrderLineItemAsync(WarehouseReceiptOrderLineDto dto)
        {
            try
            {
                if (!await ConfirmDeleteAsync(dto.Id)) return;

                if (dto.Id != Guid.Empty)
                {
                    var res = await _warehouseReceiptOrderLineService.DeleteAsync(MapToWarehouseReceiptOrderLine(dto));
                    HandleDeleteResult(res, dto);
                }
                else
                {
                    warehouseReceiptOrderLines.Remove(dto);
                }

                await RefreshGrid();
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, "Error", $"{ex.Message}{Environment.NewLine}{ex.InnerException}");
            }
        }

        private async Task<bool> ConfirmDeleteAsync(Guid id)
        {
            return await _dialogService.Confirm($"Are you sure you want to delete Receipt Order Line: {id}?", "Delete Receipt Order Line",
                new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No", AutoFocusFirstElement = true }) ?? false;
        }

        private WarehouseReceiptOrderLine MapToWarehouseReceiptOrderLine(WarehouseReceiptOrderLineDto dto)
        {
            return new WarehouseReceiptOrderLine
            {
                Id = dto.Id,
                ReceiptNo = dto.ReceiptNo,
                ProductCode = dto.ProductCode,
                UnitName = dto.UnitName,
                OrderQty = dto.OrderQty,
                TransQty = dto.TransQty,
                Bin = dto.Bin,
                LotNo = dto.LotNo,
                ExpirationDate = dto.ExpirationDate,
                Putaway = dto.Putaway,
                UnitId = dto.UnitId
            };
        }

        private void HandleDeleteResult(Result<WarehouseReceiptOrderLine> res, WarehouseReceiptOrderLineDto dto)
        {
            if (res.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, "Success", res.Messages.FirstOrDefault());
                warehouseReceiptOrderLines.Remove(dto);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, "Error", res.Messages.FirstOrDefault());
            }
        }

        private async Task RefreshDataAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(warehouseReceiptOrder.ReceiptNo))
                {
                    var res = await _warehouseReceiptOrderService.GetReceiptOrderAsync(warehouseReceiptOrder.ReceiptNo);
                    if (!res.Succeeded)
                    {
                        ShowNotification(NotificationSeverity.Error, "Error", res.Messages.ToString());
                        return;
                    }
                    warehouseReceiptOrderLines.AddRange(res.Data.WarehouseReceiptOrderLines);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, "Error", ex.Message);
            }
        }

        private async Task SyncHTData()
        {
            if (!ValidateWarehouseReceiptOrder()) return;

            try
            {
                warehouseReceiptOrder.WarehouseReceiptOrderLines = warehouseReceiptOrderLines;
                var result = await _warehouseReceiptOrderService.SyncHTData(warehouseReceiptOrder);

                if (result.Succeeded)
                {
                    UpdateWarehouseReceiptOrder(result.Data);
                    ShowNotification(NotificationSeverity.Success, "Success", "Data synced successfully");
                    await RefreshGrid();
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, "Error", result.Messages.FirstOrDefault()?.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, "Error", ex.ToString());
            }
        }

        private bool ValidateWarehouseReceiptOrder()
        {
            if (warehouseReceiptOrder == null)
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Warehouse Receipt Order information is required");
                return false;
            }

            if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Count() > 1 &&
                warehouseReceiptOrder.WarehouseReceiptOrderLines.Any(l => l.Id == Guid.Empty))
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Warehouse Receipt Order information is required");
                return false;
            }

            return true;
        }

        private void UpdateWarehouseReceiptOrder(WarehouseReceiptOrderDto updatedOrder)
        {
            warehouseReceiptOrder = updatedOrder;
            warehouseReceiptOrderLines = updatedOrder.WarehouseReceiptOrderLines.ToList();
            StateHasChanged();
        }

        private async Task InsertWarehousePutAwayOrder()
        {
            if (!await ConfirmInsertWarehousePutAwayOrder()) return;

            try
            {
                var payload = CreateWarehousePutAwayPayload();
                var res = await _warehousePutAwayServices.InsertWarehousePutAwayOrder(payload);

                if (res.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, "Success", "Created Shelved successfully");
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, "Error", res.Messages.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, "Error", ex.Message);
            }
        }

        private async Task<bool> ConfirmInsertWarehousePutAwayOrder()
        {
            if (selectedReceiptOrderLines.Count < 1)
            {
                await _dialogService.Confirm("Please select Receipt Line to create shelving", "Create Receipt",
                    new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No", AutoFocusFirstElement = true });
                return false;
            }

            return await _dialogService.Confirm("Do you want to create shelving this receipt ?", "Create Receipt",
                new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No", AutoFocusFirstElement = true }) ?? false;
        }

        private IEnumerable<WarehousePutAwayDto> CreateWarehousePutAwayPayload()
        {
            return new List<WarehouseReceiptOrderDto> { warehouseReceiptOrder }
                .Select(_ => new WarehousePutAwayDto
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    TenantId = _.TenantId,
                    DocumentNo = _.DocumentNo,
                    Location = _.Location,
                    WarehousePutAwayLines = selectedReceiptOrderLines.Select(r => new WarehousePutAwayLineDto
                    {
                        Id = r.Id,
                        ProductCode = r.ProductCode,
                        UnitId = r.UnitId,
                        JournalQty = r.OrderQty,
                        TransQty = r.TransQty,
                        Bin = r.Bin,
                        LotNo = r.LotNo
                    }),
                });
        }

        private async Task AdjustStatusReceipt(string action)
        {
            if (!await ConfirmAdjustStatusReceipt(action)) return;

            UpdateWarehouseReceiptOrderStatus(action);
            var response = await _warehouseReceiptOrderService.UpdateWarehouseReceiptOrder(warehouseReceiptOrder);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, "Success", "Successfully edited receipt");
                _navigation.NavigateTo("/warehouse-receiptlist", true);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, "Error", "Failed to edit receipt");
            }
        }

        private async Task<bool> ConfirmAdjustStatusReceipt(string action)
        {
            return await _dialogService.Confirm($"Do you want to {action} this receipt ?", "Adjust Receipt Status",
                new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No", AutoFocusFirstElement = true }) ?? false;
        }

        private void UpdateWarehouseReceiptOrderStatus(string action)
        {
            if (action.Equals(EnumReceiptStatus.Open.ToString(), StringComparison.OrdinalIgnoreCase))
                warehouseReceiptOrder.Status = EnumReceiptStatus.Open;
            else if (action.Equals(EnumReceiptStatus.Close.ToString(), StringComparison.OrdinalIgnoreCase))
                warehouseReceiptOrder.Status = EnumReceiptStatus.Close;
        }
    }
}