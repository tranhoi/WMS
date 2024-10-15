﻿using Radzen.Blazor;
using Radzen;
using Domain.Entity.WMS;

namespace WebUIFinal.Pages.ProductCategoryPage
{
    public partial class ProductCategoryList
    {
        List<ProductCategory> _dataGrid = null;
        RadzenDataGrid<ProductCategory> _profileGrid;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/addProCategory/{_localizerCommon["Detail.Create"]} {_localizer["Product Category"]}");

        async Task EditItemAsync(int id) => _navigation.NavigateTo($"/addProCategory/{_localizerCommon["Detail.Edit"]} {_localizer["Product Category"]}|{id}");
        async Task ViewItemAsync(int id)
        {

            _navigation.NavigateTo($"/addProCategory/{_localizerCommon["Detail.View"]} {_localizer["Product Category"]}|{id}");
        }

        async Task DeleteItemAsync(ProductCategory model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]} {_localizer["Product Category"]}: {model.CategoryName}?", $"{_localizerCommon["Delete"]} {_localizer["Product Category"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _productCategoryServices.DeleteAsync(model);

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
                var res = await _productCategoryServices.GetAllAsync();
                _dataGrid = null;
                _dataGrid = new List<ProductCategory>();

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
