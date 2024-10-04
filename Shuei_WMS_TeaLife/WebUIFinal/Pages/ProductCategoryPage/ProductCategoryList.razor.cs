using Radzen.Blazor;
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

            await RefreshDataAsync();
        }

        async Task AddNewItemAsync() => _navigation.NavigateTo($"/addProCategory/Create New Product Category");

        async Task EditItemAsync(int id) => _navigation.NavigateTo($"/addProCategory/Edit Product Category|{id}");
        async Task ViewItemAsync(int id)
        {

            _navigation.NavigateTo($"/addProCategory/View Product Category|{id}");
        }

        async Task DeleteItemAsync(ProductCategory model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete ProductCategory: {model.CategoryName}?", "Delete ProductCategory", new ConfirmOptions()
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
