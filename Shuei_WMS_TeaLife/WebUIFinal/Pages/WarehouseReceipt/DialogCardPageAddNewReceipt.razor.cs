using Application.DTOs.Response.Product;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;
using SupplierModel = FBT.ShareModels.Entities.Supplier;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class DialogCardPageAddNewReceipt
    {
        [Parameter] public string Title { get; set; }
        public string? ReceiptNo { get; set; }
        
        //variables
        private bool isDisabled = false;
        private bool isDisabledEditLine = false;
        private bool _showPagerSummary = true;
        private bool allowRowSelectOnRowClick = true;
        private bool _visibleBtnSubmit = true;
        private EnumReceiptOrderStatus selectedReceiptStatus = EnumReceiptOrderStatus.Draft;
        private string pagingSummaryFormat;
        private DataGridEditMode editMode = DataGridEditMode.Single;
        private bool lineIsDisable = false;

        //Master
        private WarehouseReceiptOrderDto warehouseReceiptOrder = new();
        private List<TenantAuth> tenants = new();
        private List<LocationDisplayDto> locations = new();
        private List<WarehouseReceiptOrderLineDto> warehouseReceiptOrderLines = [];
        private List<SupplierModel> suppliers = new();
        private RadzenDataGrid<WarehouseReceiptOrderLineDto>? _receiptLineProfileGrid;
        private IList<WarehouseReceiptOrderLineDto> selectedReceiptOrderLines = [];
        private List<UserDto> users = new();

        //Line
        private List<WarehouseReceiptOrderLineDto> receiptLinesToInsert = new List<WarehouseReceiptOrderLineDto>();
        private List<WarehouseReceiptOrderLineDto> receiptLinesToUpdate = new List<WarehouseReceiptOrderLineDto>();
        private IEnumerable<ProductDto> _autocompleteProducts;
        private RadzenAutoComplete _autocompleteProduct;
        private ProductDto selectedProduct = new();
        List<Bin> bins = new List<Bin>();
        Dictionary<string, ProductDto> productDict = new Dictionary<string, ProductDto>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                pagingSummaryFormat = $"{_CLoc["DisplayPage"]} {{0}} {_CLoc["Of"]} {{1}} <b>({_CLoc["Total"]} {{2}} {_CLoc["Records"]})</b>";

                SetInitialState();
                await GetReceiptOrderAsync();
                await GetTenantsAsync();
                await GetLocationsAsync();
                await GetSupplierAsync();
                await GetUserAsync();

                if (!string.IsNullOrEmpty(warehouseReceiptOrder.Location)) await OnLocationChanged(warehouseReceiptOrder.Location);
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception e)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], e.Message);
            }
        }

        private void SetInitialState()
        {
            if (Title.Contains($"{_CLoc["Detail.View"]}"))
            {
                isDisabled = true;
                isDisabledEditLine = true;
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
                ShowNotification(NotificationSeverity.Error, $"{_localizer["FailedToGetReceipt"]}", data.Messages.FirstOrDefault());
                return;
            }

            warehouseReceiptOrder = data.Data;
            selectedReceiptStatus = (EnumReceiptOrderStatus)warehouseReceiptOrder.Status;
            warehouseReceiptOrderLines.AddRange(data.Data.WarehouseReceiptOrderLines);

            await RefreshGrid();

            if (warehouseReceiptOrder.Status != EnumReceiptOrderStatus.Draft) isDisabled = true;

            if ((int)warehouseReceiptOrder.Status >= (int)EnumReceiptOrderStatus.Received) isDisabledEditLine = true;
        }

        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            if (data.Succeeded) tenants.AddRange(data.Data);
        }

        private async Task GetLocationsAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        private async Task GetSupplierAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            if (data.Succeeded) suppliers.AddRange(data.Data);
        }

        private async Task GetUserAsync()
        {
            var data = await _userToTenantServices.GetUsersAsync();
            if (data.Count > 0) users.AddRange(data);
        }

        async Task Submit(WarehouseReceiptOrderDto arg)
        {
            arg.Status = selectedReceiptStatus;

            if (Title.Contains($"{_CLoc["Detail.Create"]}"))
            {
                await CreateReceipt(arg);
            }
            else if (Title.Contains($"{_CLoc["Detail.Edit"]}"))
            {
                await UpdateReceipt(arg);
            }

            StateHasChanged();
        }

        private async Task CreateReceipt(WarehouseReceiptOrderDto arg)
        {
            if (!await ConfirmAction($"{_localizer["DoYouWantToCreateANewReceipt"]}", $"{_localizer["CreateReceipt"]}")) return;

            warehouseReceiptOrder.ReceiptNo = string.Empty;
            var response = await _warehouseReceiptOrderService.InsertWarehouseReceiptOrder(arg);

            if (response.Succeeded)
            {
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyCreatedReceipt"]}");
                _navigation.NavigateTo($"/addreceipt/{_CLoc["Detail.Edit"]} {_localizer["WarehouseReceipt"]}|{response.Data.ReceiptNo}", true);
                arg.WarehouseReceiptOrderLines.ForEach(l => l.ReceiptNo = response.Data.ReceiptNo);
            }
            else
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToCreateReceipt"]}");
            }
        }

        private async Task UpdateReceipt(WarehouseReceiptOrderDto arg)
        {
            if (!await ConfirmAction($"{_CLoc["Confirmation.Update"]} ?", $"{_localizer["UpdateReceipt"]}")) return;
            try
            {
                var response = await _warehouseReceiptOrderService.UpdateWarehouseReceiptOrder(arg);

                if (response.Succeeded)
                {
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedReceipt"]}");
                    //_navigation.NavigateTo("/warehouse-receiptlist", true);
                    arg.WarehouseReceiptOrderLines.ForEach(l => l.ReceiptNo = response.Data.ReceiptNo);
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditReceipt"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }

        }

        private async Task<bool> ConfirmAction(string message, string title)
        {
            return await _dialogService.Confirm(message, title, new ConfirmOptions
            {
                OkButtonText = _CLoc["Yes"],
                CancelButtonText = _CLoc["No"],
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

        private Task RefreshGrid() => _receiptLineProfileGrid?.RefreshDataAsync() ?? Task.CompletedTask;

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
                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SyncDataSuccessfully"]}");
                    await RefreshGrid();
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["NoDataFoundForThisReceipt"]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.ToString());
            }
        }
        private async Task CreateLineFromArrivalNo()
        {
            try
            {
                warehouseReceiptOrder.WarehouseReceiptOrderLines = warehouseReceiptOrderLines;
                var result = await _warehouseReceiptOrderService.CreateLineFromArrivalNo(warehouseReceiptOrder);

                if (result.Succeeded)
                {
                    UpdateWarehouseReceiptOrder(result.Data);
                    //ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["CreateDataSuccessfully"]}");
                    await RefreshGrid();
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer[result.Messages[0]]}");
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.ToString());
            }
        }
        private bool ValidateWarehouseReceiptOrder()
        {
            if (warehouseReceiptOrder == null)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["Confirmation.RequiredReceipt"]}");
                return false;
            }

            if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Count() > 1 &&
                warehouseReceiptOrder.WarehouseReceiptOrderLines.Any(l => l.Id == Guid.Empty))
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["Confirmation.RequiredReceipt"]}");
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

                warehouseReceiptOrder.Status = EnumReceiptOrderStatus.OnPutaway; 
                var response = await _warehouseReceiptOrderService.AdjustActionReceiptOrder(warehouseReceiptOrder);

                if (res.Succeeded && response.Succeeded) 
                {
                    UpdateWarehouseReceiptOrderStatus(EnumReceiptOrderStatus.OnPutaway.ToString());
                    selectedReceiptStatus = EnumReceiptOrderStatus.OnPutaway;

                    ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["CreatedShelvingSuccessfully"]}");
                    isDisabled = true;
                    isDisabledEditLine = true;
                }
                else
                {
                    ShowNotification(NotificationSeverity.Error, _CLoc["Error"], res.Messages.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], ex.Message);
            }
        }

        private async Task<bool> ConfirmInsertWarehousePutAwayOrder()
        {
            if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Count < 1 || warehouseReceiptOrder.WarehouseReceiptOrderLines == null)
            {
                await _dialogService.Confirm($"{_localizer["Validation.CreateShelving"]}", $"{_localizer["CreateShelving"]}",
                    new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true });
                return false;
            }
           
            return await _dialogService.Confirm($"{_localizer["Confirmation.CreateShelving"]}", $"{_localizer["CreateShelving"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
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
                        UnitId = (int)r.UnitId,
                        JournalQty = r.OrderQty,
                        TransQty = r.TransQty,
                        Bin = r.Bin,
                        LotNo = r.LotNo
                    }).ToList(),
                });
        }

        private async Task AdjustStatusReceipt(string action)
        {
            if (!await ConfirmAdjustStatusReceipt(action)) return;
            
            warehouseReceiptOrder.Status = CommonHelpers.ParseEnum<EnumReceiptOrderStatus>(action);
            var response = await _warehouseReceiptOrderService.AdjustActionReceiptOrder(warehouseReceiptOrder);

            if (response.Succeeded)
            {
                UpdateWarehouseReceiptOrderStatus(action);
                selectedReceiptStatus = (EnumReceiptOrderStatus)warehouseReceiptOrder.Status;
                ShowNotification(NotificationSeverity.Success, _CLoc["Success"], $"{_localizer["SuccessfullyEditedReceipt"]}");
            }
            else
            {
                warehouseReceiptOrder.Status = warehouseReceiptOrder.Status - 1;
                var msg = response.Messages[0];
                ShowNotification(NotificationSeverity.Error, _CLoc["Error"], $"{_localizer["FailedToEditReceipt"] + ". " +  _localizer[response.Messages[0]]}");
            }
        }

        private async Task<bool> ConfirmAdjustStatusReceipt(string action)
        {
            return await _dialogService.Confirm($"{_localizer["Confirmation.AdjustReceipt"]}", $"{_localizer["AdjustReceiptStatus"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
        }

        private void UpdateWarehouseReceiptOrderStatus(string action)
        {
            if (action.Equals(EnumReceiptOrderStatus.Open.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
            }
            else if (action.Equals(EnumReceiptOrderStatus.Received.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
                isDisabledEditLine = true;
            }
            else if (action.Equals(EnumReceiptOrderStatus.OnPutaway.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                isDisabled = true;
                isDisabledEditLine = true;
            }
        }

        private List<EnumDisplay<EnumReceiptOrderStatus>> GetDisplayReceiptOrderStatus()
        {
            return Enum.GetValues(typeof(EnumReceiptOrderStatus)).Cast<EnumReceiptOrderStatus>().Select(_ => new EnumDisplay<EnumReceiptOrderStatus>
            {
                Value = _,
                DisplayValue = GetValueReceiptOrderStatus(_)
            }).ToList();
        }

        private string GetValueReceiptOrderStatus(EnumReceiptOrderStatus receiptStatus) => receiptStatus switch
        {
            EnumReceiptOrderStatus.Draft => _localizer["Draft"],
            EnumReceiptOrderStatus.Open => _localizer["Open"],
            EnumReceiptOrderStatus.Received => _localizer["Received"],
            EnumReceiptOrderStatus.OnPutaway => _localizer["OnPutaway"],
            EnumReceiptOrderStatus.Completed => _localizer["Completed"],
            _ => throw new ArgumentException("Invalid value for ReceiptOrderStatus", nameof(receiptStatus))
        };

        private string GetStatusColor(EnumReceiptOrderStatus status) => status switch
        {
            EnumReceiptOrderStatus.Draft => "default",
            EnumReceiptOrderStatus.Open => "info",
            EnumReceiptOrderStatus.Received => "primary",
            EnumReceiptOrderStatus.OnPutaway => "error",
            EnumReceiptOrderStatus.Completed => "success",
            _ => "default",
        };

        private bool DisableCheckBoxReceiptLine(WarehouseReceiptOrderLineDto dto)
        {
            if (dto.TransQty == null || dto.TransQty < 0) return true;
            return false;
        }

        private bool VisibleCheckBoxAllReceiptLine()
        {
            if (selectedReceiptOrderLines.Where(line => line.TransQty == null || line.TransQty < 0).Select(line => line).Any()) return false;
            return true;
        }

        private async Task OnLocationChanged(string arg)
        {
            if (Guid.TryParse(warehouseReceiptOrder.Location, out Guid x))
            {
                var data = await _binServices.GetByLocationId(x);
                bins.AddRange(data.Data);
            }
        }

        #region RECEIPT LINE

        void Reset()
        {
            receiptLinesToInsert.Clear();
            receiptLinesToUpdate.Clear();
        }

        void Reset(WarehouseReceiptOrderLineDto detail)
        {
            receiptLinesToInsert.Remove(detail);
            receiptLinesToUpdate.Remove(detail);
        }

        async Task EditRow(WarehouseReceiptOrderLineDto line)
        {
          
            if (editMode == DataGridEditMode.Single && receiptLinesToInsert.Count() > 0)
            {
                Reset();
            }

            receiptLinesToUpdate.Add(line);
            await _receiptLineProfileGrid.EditRow(line);
        }

        void OnUpdateRow(WarehouseReceiptOrderLineDto line)
        {
            Reset(line);
          
        }

        async Task SaveRow(WarehouseReceiptOrderLineDto line)
        {

            if (string.IsNullOrEmpty(line.ProductCode))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["Require.ProductCode"],
                    Duration = 4000
                });
                return;
            }
            if (line.OrderQty <= 0 || line.OrderQty == null)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["Require.OrderQty"],
                    Duration = 4000
                });
                return;
            }
            if (string.IsNullOrEmpty(line.LotNo))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["Require.LotNo"],
                    Duration = 4000
                });
                return;
            }
            if (string.IsNullOrEmpty(line.Bin))
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _CLoc["Require.Bin"],
                    Duration = 4000
                });
                return;
            }
         
            if (String.IsNullOrEmpty(line.Bin) && String.IsNullOrEmpty(line.LotNo))
            { 
            bool checkProductCodeAndLot = warehouseReceiptOrder.WarehouseReceiptOrderLines.Where(_ => _.Id != line.Id && _.ProductCode == line.ProductCode && _.Bin == line.Bin && _.LotNo == line.LotNo).Any();

            if (checkProductCodeAndLot)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = string.Empty,
                    Duration = 4000
                });
                return;
            }
            }
            if (String.IsNullOrEmpty(selectedProduct.ProductCode))
            { 
                var selectedLine = warehouseReceiptOrder.WarehouseReceiptOrderLines.Where(_ => _.Id == line.Id).FirstOrDefault();
                var product = await _productServices.GetByProductCodeAsync(line.ProductCode);
                if (product != null)
                {
                    line.ProductName = product.Data.ProductName;
                    line.UnitId = product.Data.UnitId;
                    line.UnitName = product.Data.UnitName;
                    line.StockAvailableQuantity = product.Data.StockAvailableQuantity;
                }
            }
            else
            {
                line.ProductName = selectedProduct.ProductName;
                line.UnitId = selectedProduct.UnitId;
                line.UnitName = selectedProduct.UnitName;
                line.StockAvailableQuantity = selectedProduct.StockAvailableQuantity;
            }
            
            if (!String.IsNullOrEmpty(ReceiptNo))
            {
                if (line.ReceiptNo != "")
                {
                    WarehouseReceiptOrderLine receiptOrderLine = new()
                    {
                        Id = line.Id,
                        ProductCode = line.ProductCode,
                        ReceiptNo = line.ReceiptNo,
                        UnitId = line.UnitId,
                        UnitName = line.UnitName,
                        OrderQty = line.OrderQty,
                        TransQty = line.TransQty,
                        Bin = line.Bin,
                        LotNo = line.LotNo,
                        ExpirationDate = line.ExpirationDate,
                        UpdateAt = DateTime.Now,

                    };
                    var response = _warehouseReceiptOrderLineService.UpdateAsync(receiptOrderLine);
                    
                }
                else
                {
                    line.ReceiptNo = ReceiptNo;
                    line.Id = Guid.NewGuid();
                    WarehouseReceiptOrderLine receiptOrderLine = new()
                    {
                        Id = line.Id,
                        ProductCode = line.ProductCode,
                        ReceiptNo = ReceiptNo,
                        UnitId = line.UnitId,
                        UnitName = line.UnitName,
                        OrderQty = line.OrderQty,
                        TransQty = line.TransQty,
                        Bin = line.Bin,
                        LotNo = line.LotNo,
                        ExpirationDate = line.ExpirationDate,
                        CreateAt = DateTime.Now,

                    };
                    var response = _warehouseReceiptOrderLineService.InsertAsync(receiptOrderLine);
                }
                
            }
            await _receiptLineProfileGrid.UpdateRow(line);
            selectedProduct = new();
        }

        void CancelEdit(WarehouseReceiptOrderLineDto line)
        {
            Reset(line);
            _receiptLineProfileGrid.CancelEditRow(line);
        }

        async Task DeleteRow(WarehouseReceiptOrderLineDto line)
        {
            var confirm = await _dialogService.Confirm($"{_CLoc["Confirmation.Delete"]}?", $"{_CLoc["Delete"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
            if (confirm == true)
            {
                Reset(line);
                if (warehouseReceiptOrder.WarehouseReceiptOrderLines.Contains(line))
                {
                    WarehouseReceiptOrderLine deleteLine = new WarehouseReceiptOrderLine();
                    deleteLine.Id = line.Id;
                    deleteLine.ReceiptNo = line.ReceiptNo;
                    deleteLine.ProductCode = line.ProductCode;
                    _warehouseReceiptOrderLineService.DeleteAsync(deleteLine);
                    warehouseReceiptOrder.WarehouseReceiptOrderLines.Remove(line);
                    await _receiptLineProfileGrid.Reload();
                }
                else
                {
                    _receiptLineProfileGrid.CancelEditRow(line);
                    await _receiptLineProfileGrid.Reload();
                }
            }
        }

        async Task InsertRow()
        {
            if (editMode == DataGridEditMode.Single)
            {
                Reset();
            }

            var line = new WarehouseReceiptOrderLineDto();
            receiptLinesToInsert.Add(line);
            await _receiptLineProfileGrid.InsertRow(line);
        }

        void OnCreateRow(WarehouseReceiptOrderLineDto line)
        {
            warehouseReceiptOrderLines.Add(line);
            warehouseReceiptOrder.WarehouseReceiptOrderLines.Add(line);
            receiptLinesToInsert.Remove(line);
        }

        private async Task AutocompleteProduct(LoadDataArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.Filter) && args.Filter.Length >= 2)
                {
                    var result = await _productServices.SearchByProductCodeAsync(args.Filter);
                    if (result.Succeeded)
                    {
                        _autocompleteProducts = result.Data;
                    }
                    else
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = _CLoc["Error"],
                            Detail = _localizer["FailedToSearchProducts", result.Messages],
                            Duration = 4000
                        });
                    }
                }
                else
                {
                    _autocompleteProducts = null;
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _CLoc["Error"],
                    Detail = _localizer["ErrorOccurredWhileSearchingProducts", ex.Message],
                    Duration = 4000
                });
            }
        }

        private void OnProductSelected(object productCode)
        {
            productCode = _autocompleteProduct.Value;
            selectedProduct = _autocompleteProducts.FirstOrDefault(x => x.ProductCode == productCode.ToString()) ?? new();
        }
        async Task DeleteReceip()
        {
            var confirm = await _dialogService.Confirm($"{_CLoc["Confirmation.Delete"]}?", $"{_CLoc["Delete"]}",
                new ConfirmOptions { OkButtonText = _CLoc["Yes"], CancelButtonText = _CLoc["No"], AutoFocusFirstElement = true }) ?? false;
            if (confirm == true)
            {
                WarehouseReceiptOrder deletedOrder = new WarehouseReceiptOrder();
                deletedOrder.Id = warehouseReceiptOrder.Id;
               await _warehouseReceiptOrderService.DeleteAsync(deletedOrder);
               var deleteLines = new List<WarehouseReceiptOrderLine>();
                warehouseReceiptOrder.WarehouseReceiptOrderLines.ForEach(line =>
                {
                    WarehouseReceiptOrderLine deleteLine = new WarehouseReceiptOrderLine();
                    deleteLine.Id = line.Id;
                    deleteLine.ReceiptNo = line.ReceiptNo;
                    deleteLine.ProductCode = line.ProductCode;
                    deleteLines.Add(deleteLine);


                });
                await _warehouseReceiptOrderLineService.DeleteRangeAsync(deleteLines);
                _navigation.NavigateTo("/", true);
            }
        }
        #endregion
    }
}