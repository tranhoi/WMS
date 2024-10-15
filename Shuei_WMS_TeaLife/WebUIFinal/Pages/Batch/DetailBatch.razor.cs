using Application.DTOs.Response.Account;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;
using BatchEntity = Domain.Entity.WMS.Batches;

namespace WebUIFinal.Pages.Batch
{
    public partial class DetailBatch
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool isDisabled = false;
        private BatchEntity _model = new BatchEntity();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Title.Contains(_localizerCommon["Detail.View"])) isDisabled = true;

            if (Title.Contains("|"))
            {
                var sub = Title.Split('|');
                Title = sub[0];

                if (Guid.TryParse(sub[1], out Guid guid))
                {
                    Id = guid;
                }
            }

            #region Get info
            if (Id.HasValue && Id != Guid.Empty)
            {
                var arg = await _batchServices.GetByIdAsync(Id.Value);
                if (arg == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result null",
                        Duration = 1000
                    });

                    return;
                }

                _model.Id = arg.Data.Id;
                _model.ProductCode = arg.Data.ProductCode;
                _model.TenantId = arg.Data.TenantId;
                _model.LotNo = arg.Data.LotNo;
                _model.ManufacturingDate = arg.Data.ManufacturingDate;
                _model.ExpirationDate = arg.Data.ExpirationDate;
            }
            #endregion
            StateHasChanged();
        }

        async Task Submit(BatchEntity arg)
        {
            var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Save"]}", _localizerCommon["Save"], new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            if (Title.Contains(_localizerCommon["Detail.Create"])) // Add new number sequence
            {
                var res = await _batchServices.InsertAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully created",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/batches", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to create",
                        Duration = 5000
                    });
                }
            }
            else if (Title.Contains(_localizerCommon["Detail.Edit"])) // Update existing number sequence
            {
                var res = await _batchServices.UpdateAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully edited",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/batches", true);
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Failed to edit",
                        Duration = 5000
                    });
                }
            }
        }
        async Task DeleteItemAsync(BatchEntity _batch)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizerCommon["Confirmation.Delate"]}: {_batch.Id}?", _localizerCommon["Delete"], new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _batchServices.DeleteAsync(_batch);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {_batch.Id} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/device", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete {_batch.Id}.",
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
                    Detail = $"Failed to delete {_batch.Id}.",
                    Duration = 5000
                });
            }
        }
    }
}
