﻿using Radzen.Blazor;
using Radzen;
using Domain.Entity.WMS;

namespace WebUIFinal.Pages.UnitPage
{
    public partial class UnitList
    {
        List<Unit> _dataGrid = null;
        RadzenDataGrid<Unit> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/addUnit/{_localizerCommon["Detail.Create"]} {_localizer["Unit"]}");

        async Task EditItemAsync(int id) => _navigation.NavigateTo($"/addUnit/{_localizerCommon["Detail.Edit"]} {_localizer["Unit"]}|{id}");
        async Task ViewItemAsync(int id)
        {

            _navigation.NavigateTo($"/addUnit/{_localizerCommon["Detail.View"]} {_localizer["Unit"]}|{id}");
        }

        async Task DeleteItemAsync(Unit model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {model.UnitName}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _unitsService.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                    StateHasChanged();
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

                await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 5000
                });

                return;
            }
        }

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _unitsService.GetAllAsync();
                _dataGrid = null;
                _dataGrid = new List<Unit>();

                if (!res.Succeeded)
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = res.Messages.FirstOrDefault(),
                        Duration = 5000
                    });

                _dataGrid = res.Data;

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
    }
}
