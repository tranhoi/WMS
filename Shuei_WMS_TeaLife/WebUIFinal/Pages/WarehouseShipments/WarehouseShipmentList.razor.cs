using Application.DTOs;
using Application.DTOs.Request;
using Application.Extentions.Pagings;
using Radzen;

using Radzen.Blazor;

using WebUIFinal.Core;

namespace WebUIFinal.Pages.WarehouseShipments
{
    public partial class WarehouseShipmentList
    {
        private PageList<WarehouseShipmentDto> _warehouseShipments;
        private RadzenDataGrid<WarehouseShipmentDto> _warehouseShipmentGrid;
        private bool _showPagerSummary = true;
        private WarehouseShipmentSearchModel _searchModel = new WarehouseShipmentSearchModel();
        private int _count, _pageNumber = 1, _pageSize = 5;
        private IList<WarehouseShipmentDto> _selectedShipment = new List<WarehouseShipmentDto>();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await GetMasterDataAsync();
            await RefreshDataAsync();
            Constants.PagingSummaryFormat = _commonLocalizer["DisplayPage"] + " {0} " + _commonLocalizer["Of"] + " {1} <b>(" + _commonLocalizer["Total"] + " {2} " + _commonLocalizer["Records"] + ")</b>";
        }

        #region MASTER DATA
        private List<Location> _locations;
        private List<TenantAuth> _tenants;
        private List<Bin> _bins;
        private bool disabledCreatePicking = true;
        private async Task GetMasterDataAsync()
        {
            try
            {
                var locationTask = _locationServices.GetAllAsync();
                var tenantTask = _tenantsServices.GetAllAsync();
                var binTask = _binServices.GetAllAsync();

                await Task.WhenAll(locationTask, tenantTask, binTask);

                _locations = locationTask.Result.Data;
                _tenants = tenantTask.Result.Data;
                _bins = binTask.Result.Data;
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _commonLocalizer["FailedToLoadMasterData"] + ex.Message,
                    Duration = 5000
                });
            }
        }
        #endregion

        #region BUSSINESS

        void EditItemAsync(Guid shipmentId) => _navigation.NavigateTo("/addwarehouseshipment/Edit | " + shipmentId);

        void AddNewItemAsync() => _navigation.NavigateTo("/addwarehouseshipment/Create");

        async Task LoadData(LoadDataArgs args)
        {
            _pageNumber = (int)((args.Skip / args.Top) + 1);
            _pageSize = (int)args.Top;
            await RefreshDataAsync();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var model = new QueryModel<WarehouseShipmentSearchModel>
                {
                    Entity = _searchModel,
                    PageNumber = _pageNumber,
                    PageSize = _pageSize
                };
                var result = await _warehouseShipmentServices.SearchWhShipments(model);
                if (result.Succeeded)
                {
                    _warehouseShipments = result.Data;
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = _commonLocalizer["Error"],
                        Detail = result.ToString(),
                    });
                }
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        async Task CreatePickingAsync()
        {
            try
            {
                var d = new SubmitCompletedShipmentDto
                {
                    Id = _selectedShipment.Select(x => x.Id).ToList()
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
                if(res == true)
                {
                    _selectedShipment = new List<WarehouseShipmentDto>();
                    disabledCreatePicking = true;
                    await RefreshDataAsync();
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

        void CheckSelectShowHideCreatePicking(WarehouseShipmentDto data, bool isSelect = true)
        {
            if (_selectedShipment == null)
                _selectedShipment = new List<WarehouseShipmentDto>();
            var shipment = _selectedShipment.ToList();
            if(isSelect)
                shipment.Add(data);
            else
            {
                shipment = shipment.Where(x => x.Id != data.Id).ToList();
            }
            if (shipment.Count() == 0 || shipment.Select(x => x.TenantId).Distinct().Count() > 1 || shipment.Select(x => x.ShippingCarrierCode).Distinct().Count() > 1
                || shipment.Select(x => x.Status).Any(x => x != EnumShipmentOrderStatus.Open))
                disabledCreatePicking = true;
            else disabledCreatePicking = false;
        }
        #endregion
    }
}
