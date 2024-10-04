using Application.Enums;
using Domain.Entity.Commons;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;

namespace WebUIFinal.Pages.Product
{
    public partial class DialogCardPageAddNewProductJanCode
    {
        [Parameter] public ProductJanCode productJanCode { get; set; } = new ProductJanCode();
        [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

        private Status selectedStatus;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await RefreshDataAsync();

            StateHasChanged();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(productJanCode.Status))
                {
                    selectedStatus = CommonHelpers.ParseEnum<Status>(productJanCode.Status);
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

        async void Submit(ProductJanCode arg)
        {
            arg.Status = selectedStatus.ConvertToString();

            if (productJanCode.Id == 0)
            {
                var confirm = await _dialogService.Confirm($"Do you want to create a new Product Jan Code: {arg.JanCode}?", "Create Product Jan Code", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }
            else
            {
                var confirm = await _dialogService.Confirm($"Do you want to update Jan Code: {arg.JanCode}?", "Update Product Jan Code", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;
            }

            _dialogService.Close(productJanCode);
        }
    }
}
