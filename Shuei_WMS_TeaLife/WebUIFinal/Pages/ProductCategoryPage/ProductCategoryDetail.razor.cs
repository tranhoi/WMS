using Domain.Enums;
using Domain.Entity.WMS;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebUIFinal.Pages.ProductCategoryPage
{
    public partial class ProductCategoryDetail
    {
        [Parameter] public string Title { get; set; }

        private bool isDisabled = false;
        private ProductCategory _model = new ProductCategory();
        private List<string> _status = new List<string>();
        private EnumStatus _selectStatus;

        bool _visibleBtnSubmit = true, _disable = false;
        string _id = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();


            await RefreshDataAsync();
        }
        async Task RefreshDataAsync()
        {
            try
            {
                //_selectStatus = Status.Activated;

                if (Title.Contains("|"))
                {
                    if (Title.Contains(_localizerCommon["Detail.View"]))
                    {
                        _visibleBtnSubmit = false;
                        _disable = true;
                    }

                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var res = await _productCategoryServices.GetByIdAsync(int.Parse(_id));

                    if (res.Succeeded)
                    {
                        _model = res.Data;
                    }

                    //_selectStatus = Status.Activated.ToString() == _model.Status ? Status.Activated : Status.Inactivated;
                }

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


        async void Submit(ProductCategory arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}: {arg.CategoryName}?", $"{_localizerCommon["Save"]} {_localizer["Product Category"]}", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = _selectStatus;

            if (Title.Contains(_localizerCommon["Detail.Create"]))//Add
            {
                var res = await _productCategoryServices.InsertAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully created Product Category",
                        Duration = 5000
                    });
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
            else if (Title.Contains(_localizerCommon["Detail.Edit"])) //update
            {
                var res = await _productCategoryServices.UpdateAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited Product Category",
                        Duration = 5000
                    });
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
        }

        async Task DeleteItemAsync(ProductCategory ProductCategory)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delete"]}: {ProductCategory.CategoryName}?", $"{_localizerCommon["Delete"]} {_localizerCommon["Product Category"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _productCategoryServices.DeleteAsync(ProductCategory);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete ProductCategory {ProductCategory.CategoryName} successfully.",
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
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"Failed to delete ProductCategory {ProductCategory.CategoryName}.",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
