using Application.DTOs.Request.Picking;
using WebUIFinal.Core.Dto;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace WebUIFinal.Pages.WarehousePicking
{
    public partial class PickingList
    {
        [Inject]
        private ILocalStorageService _localStorage { get; set; }

        List<WarehousePickingDTO> _dataGrid = null;
        RadzenDataGrid<WarehousePickingDTO> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        bool allowRowSelectOnRowClick = false;

        PickingListSearchRequestDto _searchModel = new PickingListSearchRequestDto();

        IList<WarehousePickingDTO> _gridSelected = [];
        bool _disable = false;

        EnumShipmentOrderStatus? _selectStatus;

        List<Location> _locations = [];
        Location _locationSelect;
        List<Bin> _bins = [];
        Bin _binSelect;
        DateOnly? _planShipDateFrom, _planShipDateTo;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            _selectStatus = null;

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

            await RefreshDataAsync(_searchModel);
        }

        async Task RefreshDataAsync(PickingListSearchRequestDto model)
        {
            try
            {
                var res = await _warehousePickingListServices.GetWarehousePickingDTOAsync(model);

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
                _dataGrid = new List<WarehousePickingDTO>();
                _dataGrid = res.Data.ToList();

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
            _selectStatus = null;
            _searchModel = null;
            _planShipDateFrom = null;
            _planShipDateTo = null;
            _searchModel = new PickingListSearchRequestDto();
            await RefreshDataAsync(_searchModel);
        }

        async Task OpenAsync(WarehousePickingDTO model)
        {
            await _localStorage.SetItemAsync("PickingDetail", model);
            _navigation.NavigateTo("/pickingdetail");
        }
        async void OnSearch(PickingListSearchRequestDto arg)
        {
            var r = _gridSelected;
            arg.Location = _locationSelect?.LocationName;
            arg.Bin = _binSelect?.BinCode;
            arg.PlanShipDateFrom = _planShipDateFrom;
            arg.PlanShipDateTo = _planShipDateTo;
            arg.Status = _selectStatus;
            await RefreshDataAsync(arg);
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

        private string GetValueLocalizedStatus(EnumShipmentOrderStatus? enumStatus)
        {
            if (enumStatus.HasValue)
            {
                return _localizerEnum[enumStatus.Value.ToString()];
            }
            else
            {
                return "Status not specified";
            }
        }
    }
}
