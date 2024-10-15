using Application.DTOs.Response.Product;
using Radzen;
using Radzen.Blazor;
using WebUIFinal.Core;
using ProductModel = Domain.Entity.Commons.Product;

namespace WebUIFinal.Pages.Product
{
    public partial class ProductList
    {
        List<ProductDto> _products = new();
        RadzenDataGrid<ProductDto> _profileGrid;
        bool _showPagerSummary = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Constants.PagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();

            _filteredModel = new List<ProductDto>(_products);
        }

        async Task DeleteItemAsync(ProductModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm(_localizer["Confirmation.Delete"] + _localizer["Product"] + $": {model.ProductName}?", _localizer["Delete"]+ " " +_localizer["Product.Name"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _productServices.DeleteAsync(model);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete product {model.ProductName} successfully.",
                        Duration = 5000
                    });

                    await RefreshDataAsync();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
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
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }

        void EditItemAsync(int productId) => _navigation.NavigateTo($"/addproduct/{_localizer["Detail.Edit"]} {_localizer["Product"]}|" + productId);

        void AddNewItemAsync() => _navigation.NavigateTo($"/addproduct/{_localizer["Detail.Create"]} {_localizer["Product"]}");

        void NavigateDetailPage(int productId) => _navigation.NavigateTo($"/addproduct/{_localizer["Detail.View"]} {_localizer["Product"]}|{productId}");

        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _productServices.GetProductListAsync();

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

                _products = res.Data.ToList();
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