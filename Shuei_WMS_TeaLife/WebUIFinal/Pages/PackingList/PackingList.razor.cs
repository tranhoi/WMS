using Application.DTOs.Request.shipment;

namespace WebUIFinal.Pages.PackingList
{
    public partial class PackingList
    {
        List<WarehousePackingListDto> _dataGrid = null;
        RadzenDataGrid<WarehousePackingListDto> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool allowRowSelectOnRowClick = false;

        PackingListSearchRequestDto _searchModel = new PackingListSearchRequestDto();

        IList<WarehousePackingListDto> _gridSelected = [];
        bool _disable = false;

        DateOnly _from, _to;
        //DateOnly value = DateOnly.FromDateTime(DateTime.Now);

        EnumShipmentOrderStatus _selectStatus;

        List<Location> _locations = [];
        Location _locationSelect;
        List<Bin> _bins = [];
        Bin _binSelect;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            _selectStatus = EnumShipmentOrderStatus.All;

            var locationResponse = await _locationServices.GetAllAsync();
            if (!locationResponse.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Get location error",
                    Detail = locationResponse.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }
            _locations = locationResponse.Data.ToList();

            RefreshDataAsync(new PackingListSearchRequestDto());
        }

        async Task OpenAsync(WarehousePackingListDto model)
        {
            var m = model;

            _MasterTransferToDetails.TransferToPackingDetail = model;
            _navigation.NavigateTo("/packinglistDetail");
        }

        async Task RefreshDataAsync(PackingListSearchRequestDto model)
        {
            try
            {
                var res = await _packingListServices.GetDataMasterAsync(model);

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
                _dataGrid = new List<WarehousePackingListDto>();
                _dataGrid = res.Data.ToList();

                //await _profileGrid.RefreshDataAsync();

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

        async Task ClearFilter()
        {
            _binSelect = null;
            _locationSelect = null;
            _selectStatus = EnumShipmentOrderStatus.All;
            _searchModel = null;
            _searchModel = new PackingListSearchRequestDto();
            RefreshDataAsync(_searchModel);
        }

        async void Submit(PackingListSearchRequestDto arg)
        {
            var r = _gridSelected;
            //var m = _searchModel;
            arg.DeliveryLocation = _locationSelect?.LocationName;
            arg.OutgoingBin = _binSelect?.BinCode;
            arg.ScheduledShipDateFrom = _from.ToString("yyyy-MM-dd") == "0001-01-01" ? null : _from.ToString("yyyy-MM-dd");
            arg.ScheduledShipDateTo = _to.ToString("yyyy-MM-dd") == "0001-01-01" ? null : _to.ToString("yyyy-MM-dd");
            arg.Status = _selectStatus;
            RefreshDataAsync(arg);
        }

        async Task GetBin()
        {
            if (_locationSelect == null) return;
            var binResponse = await _binServices.GetByLocationId(_locationSelect.Id);

            if (!binResponse.Succeeded)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Get bin error",
                    Detail = binResponse.Messages.FirstOrDefault(),
                    Duration = 5000
                });
                return;
            }
            _bins = binResponse.Data.ToList();
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
    }
}
