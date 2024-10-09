using Application.DTOs;
using Application.Enums;
using Domain.Entity.authp.Commons;
using Domain.Entity.WMS.Inbound;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using System.Reflection.Metadata;
using WebUIFinal.Core;
using WebUIFinal.Core.Dto;
using SupplierModel = Domain.Entity.Commons.Supplier;

namespace WebUIFinal.Pages.WarehouseReceipt
{
    public partial class DialogCardPageAddNewReceipt
    {
        [Parameter] public string Title { get; set; }
        public string? ReceiptNo { get; set; }

        private bool isDisabled = false;
        bool _showPagerSummary = true;
        bool allowRowSelectOnRowClick = true;
        bool _visibleBtnSubmit = true;
        EnumReceiptStatus? selectedReceiptStatus;

        WarehouseReceiptOrderDto warehouseReceiptOrder = new();
        List<TenantAuth> tenants = new();
        List<LocationDisplayDto> locations = new();
        List<WarehouseReceiptOrderLineDto> warehouseReceiptOrderLines = [];
        List<SupplierModel> suppliers = new();
        RadzenDataGrid<WarehouseReceiptOrderLineDto>? _profileGrid;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                if (Title.Contains("Detail")) isDisabled = true;

                await GetReceiptOrderAsync();
                await GetTenantsAsync();
                await GetLocationssAsync();
                await GetSupplierAsync();
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

        private async Task GetReceiptOrderAsync()
        {
            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];
                ReceiptNo = sub[1];
            }

            if (!string.IsNullOrEmpty(ReceiptNo))
            {
                var data = await _warehouseReceiptOrderService.GetReceiptOrderAsync(ReceiptNo);

                if (data == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result receipt order null",
                        Duration = 1000
                    });

                    return;
                }

                warehouseReceiptOrder.Id = data.Data.Id;
                warehouseReceiptOrder.ReceiptNo = data.Data.ReceiptNo;
                warehouseReceiptOrder.Location = data.Data.Location;
                warehouseReceiptOrder.ExpectedDate = data.Data.ExpectedDate;
                warehouseReceiptOrder.TenantId = data.Data.TenantId;
                warehouseReceiptOrder.ScheduledArrivalNumber = data.Data.ScheduledArrivalNumber;
                warehouseReceiptOrder.DocumentNo = data.Data.DocumentNo;
                warehouseReceiptOrder.SupplierId = data.Data.SupplierId;
                warehouseReceiptOrder.PersonInCharge = data.Data.PersonInCharge;
                warehouseReceiptOrder.ConfirmedBy = data.Data.ConfirmedBy;
                warehouseReceiptOrder.ConfirmedDate = data.Data.ConfirmedDate;
                warehouseReceiptOrder.CreateOperatorId = data.Data.CreateOperatorId;
                warehouseReceiptOrder.CreateAt = data.Data.CreateAt;
                warehouseReceiptOrder.UpdateOperatorId = data.Data.UpdateOperatorId;
                warehouseReceiptOrder.UpdateAt = data.Data.UpdateAt;
                warehouseReceiptOrder.IsDeleted = data.Data.IsDeleted;

                if (!string.IsNullOrEmpty(data.Data.Status))
                {
                    selectedReceiptStatus = CommonHelpers.ParseEnum<EnumReceiptStatus>(data.Data.Status);
                }

                if (data.Succeeded) warehouseReceiptOrderLines.AddRange(data.Data.WarehouseReceiptOrderLines);

                if (_profileGrid != null)
                    await _profileGrid.RefreshDataAsync();
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

        async void Submit(WarehouseReceiptOrderDto arg)
        {
            arg.Status = selectedReceiptStatus.EnumConvertToString();
            warehouseReceiptOrder.Status = selectedReceiptStatus.ToString();

            arg.WarehouseReceiptOrderLines = warehouseReceiptOrderLines;

            if (Title.Contains("Create"))
            {
                var confirm = await _dialogService.Confirm($"Do you want to create a new receipt ?", "Create Receipt", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                warehouseReceiptOrder.ReceiptNo = string.Empty;

                var response = await _warehouseReceiptOrderService.InsertWarehouseReceiptOrder(arg);

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully created product",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/warehouse-receiptlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to create product",
                        Duration = 5000
                    });
                }
            }

            if (Title.Contains("Edit"))
            {
                var confirm = await _dialogService.Confirm($"Do you want to update ?", "Create Receipt", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var response = await _warehouseReceiptOrderService.UpdateWarehouseReceiptOrder(arg);

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited receipt",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/warehouse-receiptlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to edit receipt",
                        Duration = 5000
                    });
                }
            }
            _dialogService.Close("Success");
        }

        async Task AddReceiptOrderLine()
        {
            try
            {
                WarehouseReceiptOrderLineDto warehouseReceiptOrderLineInfor = new()
                {
                    ReceiptNo = warehouseReceiptOrder.ReceiptNo,
                };

                var res = await _dialogService.OpenAsync<DialogCardPageAddNewReceiptLine>($"Create new Receipt Order Line",
                        new Dictionary<string, object>() { { "warehouseReceiptOrderLine", warehouseReceiptOrderLineInfor }, { "VisibleBtnSubmit", true } },
                        new DialogOptions()
                        {
                            Width = "800",
                            Height = "400",
                            Resizable = true,
                            Draggable = true,
                            CloseDialogOnOverlayClick = true
                        });


                if (res != null)
                {
                    var selectResult = (WarehouseReceiptOrderLineDto)res;

                    var returnModel = warehouseReceiptOrderLines.FirstOrDefault(x => x.Id == selectResult.Id);

                    if (returnModel?.ProductCode == selectResult.ProductCode && returnModel?.LotNo == selectResult.LotNo)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = $"{selectResult.Id} has been exist.",
                            Duration = 5000
                        });
                        return;
                    }

                    var product = await _productServices.GetByProductCodeAsync(selectResult.ProductCode);

                    if (product != null)
                    {
                        selectResult.UnitName = product.Data.UnitName;
                        selectResult.ProductName = product.Data.ProductName;
                        selectResult.StockAvailableQuantity = product.Data.StockAvailableQuantity;
                        selectResult.ReceiptNo = string.Empty;
                    }

                    warehouseReceiptOrderLines.Add(selectResult);

                    await _profileGrid.RefreshDataAsync();
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                    Duration = 5000
                });

                return;
            }
        }

        async Task ViewReceiptOrderLineItemAsync(WarehouseReceiptOrderLineDto dto)
        {
            var res = await _dialogService.OpenAsync<DialogCardPageAddNewReceiptLine>($"Create new Recepit Order Line",
                    new Dictionary<string, object>() { { "warehouseReceiptOrderLine", dto }, { "VisibleBtnSubmit", false } },
                    new DialogOptions()
                    {
                        Width = "800",
                        Height = "400",
                        Resizable = true,
                        Draggable = true,
                        CloseDialogOnOverlayClick = true
                    });

            if (res == "Success")
            {
                await RefreshDataAsync();
            }
        }
        async Task EditReceiptOrderLineItemAsync(WarehouseReceiptOrderLineDto model)
        {
            var res = await _dialogService.OpenAsync<DialogCardPageAddNewReceiptLine>($"Edit Recepit Order Line ",
                    new Dictionary<string, object>() { { "warehouseReceiptOrderLine", model }, { "VisibleBtnSubmit", true } },
                    new DialogOptions()
                    {
                        Width = "1000",
                        Height = "400",
                        Resizable = true,
                        Draggable = true,
                        CloseDialogOnOverlayClick = true
                    });

            if (_profileGrid != null)
                await _profileGrid.RefreshDataAsync();
        }

        async Task DeleteReceiptOrderLineItemAsync(WarehouseReceiptOrderLineDto dto)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete Recepit Order Line: {dto.Id}?", "Delete Recepit Order Line", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                if (dto.Id != Guid.Empty)
                {
                    var res = await _warehouseReceiptOrderLineService.DeleteAsync(new WarehouseReceiptOrderLine 
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
                    });

                    if (res.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = res.Messages.FirstOrDefault(),
                            Duration = 5000
                        });

                        warehouseReceiptOrderLines.Remove(dto);
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
                else
                {
                    warehouseReceiptOrderLines.Remove(dto);
                }

                if (_profileGrid != null)
                    await _profileGrid.RefreshDataAsync();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"{ex.Message}{Environment.NewLine}{ex.InnerException}",
                    Duration = 5000
                });

                return;
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(warehouseReceiptOrder.ReceiptNo))
                {
                    var res = await _warehouseReceiptOrderService.GetReceiptOrderAsync(warehouseReceiptOrder.ReceiptNo);

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
                    warehouseReceiptOrderLines.AddRange(res.Data.WarehouseReceiptOrderLines);
                }

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
    }
}
