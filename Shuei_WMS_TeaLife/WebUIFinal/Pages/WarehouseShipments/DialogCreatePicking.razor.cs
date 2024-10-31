using Radzen;
using System.Web.Mvc;
using Application.DTOs;
using Microsoft.AspNetCore.Components;

namespace WebUIFinal.Pages.WarehouseShipments;

public partial class DialogCreatePicking
{

    [Parameter] public SubmitCompletedShipmentDto _model { get; set; } = new SubmitCompletedShipmentDto();
    [Parameter] public bool VisibleBtnSubmit { get; set; } = true;

    private List<SelectListItem> _personInChargeList;

    private async Task GetMasterDataAsync()
    {
        try
        {
            var personInChargeTask = _categoriesService.GetUserDropdown();

            await Task.WhenAll(personInChargeTask);

            _personInChargeList = personInChargeTask.Result.Data;
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["FailedToLoadMasterData", ex.Message],
                Duration = 5000
            });
        }
    }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetMasterDataAsync();
        //StateHasChanged();
    }
    
    async Task Submit(SubmitCompletedShipmentDto arg)
    {
        var confirm = await _dialogService.Confirm($"{_shipmentLocalizer["Create.Picking"]}?", $"{_commonLocalizer["Create"]} {_shipmentLocalizer["Picking"]}", new ConfirmOptions()
        {
            OkButtonText = _commonLocalizer["Yes"],
            CancelButtonText = _commonLocalizer["No"],
            AutoFocusFirstElement = true,
        });

        if (confirm == null || confirm == false) return;

        try
        {
            var response = await _warehouseShipmentServices.CreatePickingAsync(arg);
            if (response.Succeeded)
            {
                _dialogService.Close(true);
            }
            else
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = _commonLocalizer["Error"],
                    Detail = _shipmentLocalizer["FailedToCreatePicking", response.Messages.FirstOrDefault()],
                    Duration = 5000
                });
            }
        }
        catch (Exception ex)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = _commonLocalizer["Error"],
                Detail = _shipmentLocalizer["ErrorOccurredWhileCreatingPicking", ex.Message],
                Duration = 5000
            });
        }
    }
}
