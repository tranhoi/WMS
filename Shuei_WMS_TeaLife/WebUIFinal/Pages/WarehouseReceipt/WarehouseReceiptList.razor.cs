using Application.DTOs;
using Domain.Entity.WMS;
using Domain.Entity.WMS.Inbound;
using Radzen;
using Radzen.Blazor;
using SupplierModel = Domain.Entity.Commons.Supplier;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class WarehouseReceiptList
    {
        List<WarehouseReceiptOrderDto> receiptOrders = new();
        List<SupplierModel> suppliers = new();
        List<Location> locations = new();
        RadzenDataGrid<WarehouseReceiptOrderDto> _profileGrid;
        IList<WarehouseReceiptOrderDto> _selectedReceiptOrders = [];
        bool _showPagerSummary = true;
        bool allowRowSelectOnRowClick = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                await GetSupplierAsync();
                await RefreshDataAsync();
                await GetLocationAsync();

                _filteredModel = new List<WarehouseReceiptOrderDto>(receiptOrders);
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

        private async Task GetSupplierAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            suppliers.AddRange(data.Data);
        }

        private async Task GetLocationAsync()
        {
            var data = await _locationServices.GetAllAsync();
            locations.AddRange(data.Data);
        }

        void EditItemAsync(string receiptNo) => _navigation.NavigateTo("/addreceipt/Edit Warehouse Receipt|" + receiptNo);

        void AddNewItemAsync() => _navigation.NavigateTo("/addreceipt/Create Warehouse Receipt");

        void NavigateDetailPage(string receiptNo) => _navigation.NavigateTo($"/addreceipt/Warehouse Receipt Detail|{receiptNo}");

        async Task DeleteItemAsync(WarehouseReceiptOrderDto model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete reciept: {model.Id}?", "Delete product", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });
                if (confirm == null || confirm == false) return;

                var res = await _warehouseReceiptOrderService.DeleteAsync(new WarehouseReceiptOrder { Id = model.Id, ReceiptNo = model.ReceiptNo, Location = model.Location, TenantId = model.TenantId, SupplierId = model.SupplierId });
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete receipt {model.Id} successfully.",
                        Duration = 5000
                    });

                    receiptOrders.Remove(model);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages?.FirstOrDefault()?.ToString(),
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
                    Detail = ex.Message?.FirstOrDefault().ToString(),
                    Duration = 5000
                });
            }

            if (_profileGrid != null)
                await _profileGrid.RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _warehouseReceiptOrderService.GetReceiptOrderListAsync();

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }

                receiptOrders = res.Data.ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        async Task InsertWarehousePutAwayOrder()
        {
            try
            {
                var payload = _selectedReceiptOrders.Select(_ => new WarehousePutAwayDto
                {
                    Id = _.Id,
                    ReceiptNo = _.ReceiptNo,
                    TenantId = _.TenantId,
                    DocumentNo = _.DocumentNo,
                    Location = _.Location,
                    WarehousePutAwayLines = _.WarehouseReceiptOrderLines.Select(r => new WarehousePutAwayLineDto
                    {
                        Id = r.Id,
                        ProductCode = r.ProductCode,
                        UnitId = r.UnitId,
                        JournalQty = r.OrderQty,
                        TransQty = r.TransQty,
                        Bin = r.Bin,
                        LotNo = r.LotNo
                    }),
                }).AsEnumerable();

                var res = await _warehousePutAwayServices.InsertWarehousePutAwayOrder(payload);

                if (!res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.ToString(),
                    });
                    return;
                }

                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Created Shelved successfully",
                    Duration = 5000
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
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
