using Application.DTOs.Response.Product;
using Microsoft.AspNetCore.Components;
using System.Web.Mvc;

namespace WebUIFinal.Pages.WarehouseShipments;
public partial class DialogCardPageAddNewWarehouseShipment
{
    public string Title { get; set; }
    [Parameter]
    public string Mode { get; set; }
    private WarehouseShipmentDto model = new WarehouseShipmentDto();
    private bool isDisabled = false;
    private List<Location> locations;
    private List<FBT.ShareModels.Entities.Product> products;
    private List<FBT.ShareModels.WMS.ShippingCarrier> shippingCarriers;

    #region MASTER DATA
    private List<SelectListItem> _locations;
    private List<TenantAuth> _tenants;
    private List<Bin> _bins, _allBins;
    private List<FBT.ShareModels.Entities.Product> _products;
    private List<FBT.ShareModels.WMS.ShippingCarrier> _shippingCarriers;
    private List<SelectListItem> _personInChargeList;
    private IEnumerable<ProductDto> _autocompleteProducts;
    private RadzenAutoComplete _autocompleteProduct;
    private string productName;
    private ProductDto selectedProduct = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetMasterDataAsync();
        await InitializeModel();
    }

    private async Task GetMasterDataAsync()
    {
        try
        {
            var locationTask = _locationServices.GetAllAsync();
            var tenantTask = _tenantsServices.GetAllAsync();
            var binTask = _binServices.GetAllAsync();
            var productTask = _productServices.GetAllAsync();
            var shippingCarrierTask = _shippingCarrierServices.GetAllAsync();
            var personInChargeTask = _categoriesService.GetUserDropdown();

            await Task.WhenAll(locationTask, tenantTask, binTask, productTask, shippingCarrierTask, personInChargeTask);

            _locations = locationTask.Result.Data.Select(x => new SelectListItem
            {
                Text = x.LocationName,
                Value = x.Id.ToString()
            }).ToList();
            _tenants = tenantTask.Result.Data;
            _allBins = _bins = binTask.Result.Data;
            _products = productTask.Result.Data;
            _shippingCarriers = shippingCarrierTask.Result.Data;
            _personInChargeList = personInChargeTask.Result.Data;
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _commonLocalizer["FailedToLoadMasterData", ex.Message],
                Duration = 5000
            });
        }
    }

    private void OnChangeLocation(object locationId)
    {
        _bins = _allBins.Where(x => x.LocationId == Guid.Parse(locationId.ToString())).ToList();
    }
    #endregion

    private async Task InitializeModel()
    {
        if (Mode.StartsWith("Edit"))
        {
            var sub = Mode.Split('|');
            var shipmentId = sub[1];
            var result = await _warehouseShipmentServices.GetShipmentByIdAsync(Guid.Parse(shipmentId));
            if (result.Succeeded)
            {
                model = result.Data;
                _bins = _allBins.Where(x => x.LocationId == Guid.Parse(model.Location)).ToList();
            }
            Title = _shipmentLocalizer["EditWarehouseShipment"];
            isDisabled = model.Status >= FBT.ShareModels.EnumShipmentOrderStatus.Open;
        }
        else
        {
            Title = _shipmentLocalizer["CreateWarehouseShipment"];
            var sequenceShipment = await _numberSequenceServices.GetNumberSequenceByType("Shipment");
            if (sequenceShipment.Succeeded)
            {
                model.ShipmentNo = $"{sequenceShipment.Data.Prefix}{sequenceShipment.Data.CurrentSequenceNo.ToString().PadLeft((int)sequenceShipment.Data.SequenceLength, '0')}";
            }
        }
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
                        Summary = _commonLocalizer["Error"],
                        Detail = _shipmentLocalizer["FailedToSearchProducts", result.Messages],
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
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ErrorOccurredWhileSearchingProducts", ex.Message],
                Duration = 4000
            });
        }
    }

    private void OnProductSelected(object productCode)
    {
        productCode = _autocompleteProduct.Value;
        selectedProduct = _autocompleteProducts.FirstOrDefault(x => x.ProductCode == productCode.ToString()) ?? new();
    }

    private async Task Submit(WarehouseShipmentDto model)
    {
        if (model.WareHouseShipmentLineDtos.Count == 0)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["PleaseEnterDetail"],
                Duration = 4000
            });
        }
        else
        {
            #region MAPPING NAME
            var location = _locations.FirstOrDefault(x => x.Value == model.Location);
            if (location != null)
            {
                model.LocationName = location.Text;
            }

            var tenant = _tenants.FirstOrDefault(x => x.TenantId == model.TenantId);
            if (tenant != null)
            {
                model.TenantName = tenant.TenantFullName;
            }
            var personInCharge = _personInChargeList.FirstOrDefault(x => x.Value == model.PersonInCharge);
            if (personInCharge != null)
            {
                model.PersonInChargeName = personInCharge.Text;
            }
            #endregion

            if (Mode == "Create")
            {
                var result = await _warehouseShipmentServices.CreateWarehouseShipmentAsync(model);
                if (result.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _commonLocalizer["Success"],
                        Detail = _shipmentLocalizer["WarehouseShipmentCreatedSuccessfully"],
                        Duration = 4000
                    });
                    _navigation.NavigateTo("/ShippingInstructions");
                }
            }
            else if (Mode.StartsWith("Edit"))
            {
                var result = await _warehouseShipmentServices.UpdateWarehouseShipmentAsync(model);
                if (result.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = _commonLocalizer["Success"],
                        Detail = _shipmentLocalizer["WarehouseShipmentUpdatedSuccessfully"],
                        Duration = 4000
                    });
                    //_navigation.NavigateTo("/ShippingInstructions");
                }
            }
        }
    }

    async Task DeleteItemAsync(Guid id)
    {
        try
        {
            var confirm = await _dialogService.Confirm(_commonLocalizer["Confirmation.Delete"] + _shipmentLocalizer["WarehouseShipment"] + "?", _commonLocalizer["Delete"] + " " + _shipmentLocalizer["WarehouseShipment.ShipmentNo"], new ConfirmOptions()
            {
                OkButtonText = _commonLocalizer["Yes"],
                CancelButtonText = _commonLocalizer["No"],
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;
            var res = await _warehouseShipmentServices.DeleteShipmentAsync(id);

            if (res.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _commonLocalizer["Success"],
                    Detail = _shipmentLocalizer["WarehouseShipmentDeletedSuccessfully"],
                    Duration = 4000
                });
                _navigation.NavigateTo("/ShippingInstructions");
            }
            else
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = res.Messages.ToString(),
                    Duration = 5000
                });
            }
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = ex.Message,
                Duration = 5000
            });
        }
    }
    
    private async Task ConfirmShipment()
    {
        var confirm = await _dialogService.Confirm(_shipmentLocalizer["ConfirmWarehouseShipmentConfirmation"], _commonLocalizer["UpdateConfirmation"], new ConfirmOptions() { OkButtonText = _commonLocalizer["Yes"], CancelButtonText = _commonLocalizer["No"] });
        if (confirm == true)
        {
            var result = await _warehouseShipmentServices.ConfirmShipmentAsync(model.Id);
            if (result.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = _commonLocalizer["Success"],
                    Detail = _shipmentLocalizer["WarehouseShipmentConfirmedSuccessfully"],
                    Duration = 4000
                });
                _navigation.NavigateTo("/ShippingInstructions");
            }
        }
    }

    //detail
    RadzenDataGrid<WarehouseShipmentLineDto> shipmentDetailsGrid;
    DataGridEditMode editMode = DataGridEditMode.Single;

    List<WarehouseShipmentLineDto> shipmentDetailsToInsert = new List<WarehouseShipmentLineDto>();
    List<WarehouseShipmentLineDto> shipmentDetailsToUpdate = new List<WarehouseShipmentLineDto>();

    void Reset()
    {
        shipmentDetailsToInsert.Clear();
        shipmentDetailsToUpdate.Clear();
    }

    void Reset(WarehouseShipmentLineDto detail)
    {
        shipmentDetailsToInsert.Remove(detail);
        shipmentDetailsToUpdate.Remove(detail);
    }

    async Task EditRow(WarehouseShipmentLineDto detail)
    {
        if (editMode == DataGridEditMode.Single && shipmentDetailsToInsert.Count() > 0)
        {
            Reset();
        }
        selectedProduct = new ProductDto
        {
            ProductCode = detail.ProductCode,
            ProductName = detail.ProductName,
            UnitId = (int)detail.UnitId,
            UnitName = detail.Unit,
            StockAvailableQuantityTrans = detail.StockAvailable,
            QuantityShipment = detail.StockAvailable - detail.AvailableQuantity
        };
        shipmentDetailsToUpdate.Add(detail);
        await shipmentDetailsGrid.EditRow(detail);
    }

    void OnUpdateRow(WarehouseShipmentLineDto detail)
    {
        Reset(detail);
        // Update logic here
    }

    async Task SaveRow(WarehouseShipmentLineDto detail)
    {
        if (string.IsNullOrEmpty(detail.ProductCode))
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ProductIsRequired"],
                Duration = 4000
            });
            return;
        }
        if (string.IsNullOrEmpty(detail.Bin))
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["BinIsRequired"],
                Duration = 4000
            });
            return;
        }
        //check exist productcode & bin
        if (CheckExistLine(detail))
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ProductIsExisted"],
                Duration = 4000
            });
            return;
        }
        if (detail.ShipmentQty == 0 || detail.ShipmentQty == default)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ShipmentQtyIsRequired"],
                Duration = 4000
            });
            return;
        }
        detail.Id = Guid.NewGuid();
        detail.ProductName = selectedProduct.ProductName;
        detail.Unit = selectedProduct.UnitName;
        detail.StockAvailable = (int)selectedProduct.StockAvailableQuantityTrans;
        detail.AvailableQuantity = (int)(selectedProduct.StockAvailableQuantityTrans - selectedProduct.QuantityShipment);
        detail.PackedQty = 0;
        detail.UnitId = selectedProduct.UnitId;
        detail.Location = model.Location;
        detail.ShipmentNo = model.ShipmentNo;
        await shipmentDetailsGrid.UpdateRow(detail);
        selectedProduct = new();
    }
    private bool CheckExistLine(WarehouseShipmentLineDto detail)
    {
        return model.WareHouseShipmentLineDtos.Any(line => 
            line.Id != detail.Id &&
            line.ProductCode == detail.ProductCode && 
            line.Bin == detail.Bin);
    }
    void CancelEdit(WarehouseShipmentLineDto detail)
    {
        Reset(detail);
        shipmentDetailsGrid.CancelEditRow(detail);
    }

    async Task DeleteRow(WarehouseShipmentLineDto detail)
    {
        var confirm = await _dialogService.Confirm(_shipmentLocalizer["DeleteWarehouseShipmentProductConfirmation"], _commonLocalizer["DeleteConfirmation"], new ConfirmOptions() { OkButtonText = _commonLocalizer["Yes"], CancelButtonText = _commonLocalizer["No"] });
        if (confirm == true)
        {
            Reset(detail);
            if (model.WareHouseShipmentLineDtos.Contains(detail))
            {
                model.WareHouseShipmentLineDtos.Remove(detail);
                await shipmentDetailsGrid.Reload();
            }
            else
            {
                shipmentDetailsGrid.CancelEditRow(detail);
                await shipmentDetailsGrid.Reload();
            }
        }
        
    }
    
    async Task InsertRow()
    {
        if (editMode == DataGridEditMode.Single)
        {
            Reset();
        }

        var detail = new WarehouseShipmentLineDto()
        {
            Bin = model.BinId
        };
        shipmentDetailsToInsert.Add(detail);
        await shipmentDetailsGrid.InsertRow(detail);
    }

    void OnCreateRow(WarehouseShipmentLineDto detail)
    {
        model.WareHouseShipmentLineDtos.Add(detail);
        shipmentDetailsToInsert.Remove(detail);
    }

    async Task CreatePickingAsync()
    {
        try
        {

            var d = new SubmitCompletedShipmentDto
            {
                Id = new List<Guid> { model.Id }
            };
            var sequencePicking = await _numberSequenceServices.GetNumberSequenceByType("Picking");
            if (sequencePicking.Succeeded)
            {
                d.PickingNo = $"{sequencePicking.Data.Prefix}{sequencePicking.Data.CurrentSequenceNo.ToString().PadLeft((int)sequencePicking.Data.SequenceLength, '0')}";
            }
            var res = await _dialogService.OpenAsync<DialogCreatePicking>($"{_commonLocalizer["Detail.View"]} {_shipmentLocalizer["Picking"]}",
               new Dictionary<string, object>() { { "_model", d }, { "VisibleBtnSubmit", true } },
               new DialogOptions()
               {
                   Width = "800",
                   Height = "400",
                   Resizable = true,
                   Draggable = true,
                   CloseDialogOnOverlayClick = true
               });
            if (res == true)
            {
                _navigation.NavigateTo("/ShippingInstructions");
            }
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                Duration = 5000
            });

            return;
        }
    }
}