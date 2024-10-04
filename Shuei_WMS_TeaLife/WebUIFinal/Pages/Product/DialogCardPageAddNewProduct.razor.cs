using Application.DTOs.Request.Products;
using Application.Enums;
using Domain.Entity.authp.Commons;
using Domain.Entity.Commons;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using Radzen.Blazor;
using WebUIFinal.Core.Dto;
using ProductCategoryModel = Domain.Entity.WMS.ProductCategory;
using ProductModel = Domain.Entity.Commons.Product;
using SupplierModel = Domain.Entity.Commons.Supplier;
using UnitModel = Domain.Entity.WMS.Unit;

namespace WebUIFinal.Pages.Product
{
    public partial class DialogCardPageAddNewProduct
    {
        [Parameter] public string Title { get; set; }
        public int? ProductId { get; set; }

        private EnumProductStatus selectedStatus;
        private EnumProductType selectedProductType;
        private bool isDisabled = false;
        private string? imageBase64String;
        bool _showPagerSummary = true;
        bool allowRowSelectOnRowClick = true;
        bool _visibleBtnSubmit = true;

        ProductModel model = new ProductModel();
        List<SupplierModel> suppliers = new();
        List<UnitModel> units = new();
        List<TenantAuth> tenants = new();
        List<ProductCategoryModel> productCategories = new();
        List<ProductJanCode> productJanCodes = [];
        IList<ProductJanCode> _selectedJanCodes = [];

        RadzenDataGrid<ProductJanCode>? _productJanCodeProfileGrid;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await base.OnInitializedAsync();

                if (Title.Contains("Detail")) isDisabled = true;

                await GetProductDetail();
                await GetUnitsAsync();
                await GetSupplierAsync();
                await GetTenantsAsync();
                await GetProductCategoryAsync();
                await GetProductJanCodesAsync();
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

        private async Task GetProductCategoryAsync()
        {
            var data = await _productCategoryServices.GetAllAsync();
            productCategories.AddRange(data.Data.Where(_ => _.IsDeleted == false));
        }

        private async Task GetSupplierAsync()
        {
            var data = await _suppliersServices.GetAllAsync();
            suppliers.AddRange(data.Data);
        }

        private async Task GetTenantsAsync()
        {
            var data = await _tenantsServices.GetAllAsync();
            tenants.AddRange(data.Data);
        }

        private async Task GetUnitsAsync()
        {
            var data = await _unitsService.GetAllAsync();
            units.AddRange(data.Data.Where(_ => _.IsDeleted == false));
        }

        private async Task GetProductDetail()
        {
            #region Get product info
            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Int32.TryParse(sub[1], out int x)) 
                {
                    ProductId = x;
                }
            }

            if (ProductId.HasValue && ProductId > 0)
            {
                var product = await _productServices.GetByIdAsync((int)ProductId);
                if (product == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result product null",
                        Duration = 1000
                    });

                    return;
                }

                model.Id = product.Data.Id;
                model.ProductCode = product.Data.ProductCode;
                model.ProductName = product.Data.ProductName;
                model.ProductShortCode = product.Data.ProductShortCode;
                model.ProductImageUrl = product.Data.ProductImageUrl;
                model.MakerManagementCode = product.Data.MakerManagementCode;
                model.Description = product.Data.Description;
                model.Height = product.Data.Height;
                model.Weight = product.Data.Weight;
                model.Depth = product.Data.Depth;
                model.Currency = product.Data.Currency;
                model.ProductUrl = product.Data.ProductUrl;
                model.StandardPrice = product.Data.StandardPrice;
                model.WarehouseProcessingFlag = product.Data.WarehouseProcessingFlag;
                model.UnitId = product.Data.UnitId;
                model.TenantId = product.Data.TenantId;
                model.SupplierId = product.Data.SupplierId;
                model.CategoryId = product.Data.CategoryId;
                model.ShippingLimitDays = product.Data.ShippingLimitDays;
                model.ProductImageName = product.Data.ProductImageName;

                selectedStatus = (EnumProductStatus)product.Data.ProductStatus;
                selectedProductType = (EnumProductType)product.Data.ProductType;

                imageBase64String = model?.ProductImageName;
            }

            #endregion
        }

        private async Task GetProductJanCodesAsync()
        {
            if (ProductId.HasValue)
            {
                var data = await _productJanCodeService.GetByProductId((int)ProductId);

                if (data.Succeeded)
                {
                    productJanCodes.AddRange(data.Data);
                }

                if (_productJanCodeProfileGrid != null)
                    await _productJanCodeProfileGrid.RefreshDataAsync();
            }
        }

        async void Submit(ProductModel arg)
        {
            if (Title.Contains("Create"))
            {
                var confirm = await _dialogService.Confirm($"Do you want to create a new product: {arg.ProductName}?", "Create product", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                model.ProductType = (int)selectedProductType;
                model.ProductStatus = (int)selectedStatus;

                var response = await _productServices.InsertAsync(arg);

                if (productJanCodes != null)
                {
                    productJanCodes.ForEach(_ => _.ProductId = response.Data.Id);

                    await _productJanCodeService.AddRangeAsync(productJanCodes);
                }

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully created product",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/productlist", true);
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
                var confirm = await _dialogService.Confirm($"Do you want to update product: {arg.ProductName}?", "Create product", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                model.ProductType = (int)selectedProductType;
                model.ProductStatus = (int)selectedStatus;
                  
                var response = await _productServices.UpdateAsync(model);

                if (productJanCodes != null && ProductId.HasValue)
                {
                    productJanCodes.ForEach(_ => _.ProductId = (int)ProductId);

                    await _productJanCodeService.AddOrUpdateAsync(productJanCodes);
                }

                if (response.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited product",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/productlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to edit product",
                        Duration = 5000
                    });
                }
            }
            _dialogService.Close("Success");
        }

        async Task DeleteItemAsync(ProductModel model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete product: {model.ProductName}?", "Delete product", new ConfirmOptions()
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

                    _navigation.NavigateTo("/productlist", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete product {model.ProductName}.",
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

        async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            var payload = new ProductRequestDTO();

            if (string.IsNullOrEmpty(model.ProductCode))
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"The product code is required to upload image. Please enter the product code !",
                    Duration = 5000
                });

                return;
            }

            // Only handle image files
            if (file != null && file.ContentType.StartsWith("image/"))
            {
                var buffer = new byte[file.Size];
                await file.OpenReadStream().ReadAsync(buffer);
                imageBase64String = Convert.ToBase64String(buffer);
                payload.ProductImage = Convert.ToBase64String(buffer);
                payload.ProductCode = model.ProductCode;
            }

            var imageResult = await _productServices.UploadProductImage(payload);

            if (!string.IsNullOrEmpty(imageResult.Data)) 
            {
                model.ProductImageName = imageResult.Data;
            }
        }

        void HandleRemoveFile()
        {
            if (string.IsNullOrEmpty(imageBase64String))
            {
                return;
            }

            imageBase64String = null;
            model.ProductImageName = null;

            StateHasChanged();
        }

        private List<EnumDisplay<EnumProductType>> GetDisplayProductType()
        {
            return Enum.GetValues(typeof(EnumProductType)).Cast<EnumProductType>().Select(_ => new EnumDisplay<EnumProductType>
            {
                Value = _,
                DisplayValue = GetValueProductType(_)
            }).ToList();
        }

        private string GetValueProductType(EnumProductType productType) => productType switch
        {
            EnumProductType.SingleItem => "Single Item",
            EnumProductType.FixedSet => "Fixed Set",
            EnumProductType.MonthlyChangeSet => "Monthly Change Set",
            _ => throw new ArgumentException("Invalid value for productType", nameof(productType))
        };

        async Task AddProductJanCode()
        {
            ProductJanCode janInfor = new ProductJanCode()
            {
                ProductId = model.Id
            };

            var res = await _dialogService.OpenAsync<DialogCardPageAddNewProductJanCode>($"Create new Product Jan Code",
                    new Dictionary<string, object>() { { "productJanCode", janInfor }, { "VisibleBtnSubmit", true } },
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
                var selectResult = (ProductJanCode)res;

                var returnModel = productJanCodes.FirstOrDefault(x => x.JanCode == selectResult.JanCode);

                if (returnModel != null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"{selectResult.JanCode} has been exist.",
                        Duration = 5000
                    });
                    return;
                }

                productJanCodes.Add(selectResult);

                if (_productJanCodeProfileGrid != null)
                    await _productJanCodeProfileGrid.RefreshDataAsync();
            }
        }

        async Task ViewJanCodeItemAsync(ProductJanCode dto)
        {
            var res = await _dialogService.OpenAsync<DialogCardPageAddNewProductJanCode>($"Create new Product Jan Code",
                    new Dictionary<string, object>() { { "productJanCode", dto }, { "VisibleBtnSubmit", false } },
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
        async Task EditJanCodeItemAsync(ProductJanCode model)
        {
            var res = await _dialogService.OpenAsync<DialogCardPageAddNewProductJanCode>($"Edit Product Jan Code ",
                    new Dictionary<string, object>() { { "productJanCode", model }, { "VisibleBtnSubmit", true } },
                    new DialogOptions()
                    {
                        Width = "1000",
                        Height = "400",
                        Resizable = true,
                        Draggable = true,
                        CloseDialogOnOverlayClick = true
                    });

            if (_productJanCodeProfileGrid != null)
                await _productJanCodeProfileGrid.RefreshDataAsync();
        }

        async Task DeleteJanCodeItemAsync(ProductJanCode dto)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete Product Jan Code: {dto.JanCode}?", "Delete jan code", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                if (dto.Id != 0) 
                {
                    var res = await _productJanCodeService.DeleteAsync(dto);

                    if (res.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = res.Messages.FirstOrDefault(),
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
                            Detail = res.Messages.FirstOrDefault(),
                            Duration = 5000
                        });
                    }
                }
                else
                {
                    productJanCodes.Remove(dto);
                }

                if (_productJanCodeProfileGrid != null)
                    await _productJanCodeProfileGrid.RefreshDataAsync();
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
                if (ProductId.HasValue)
                {
                    var res = await _productJanCodeService.GetByProductId((int)ProductId);

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

                    productJanCodes = res.Data.ToList();
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