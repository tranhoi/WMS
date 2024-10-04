using Application.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;
using NumberSequenceEntity = Domain.Entity.WMS.NumberSequences;

namespace WebUIFinal.Pages.NumberSequence
{
    public partial class NumberSequenceDetail
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool isDisabled = false;
        private NumberSequenceEntity _model = new NumberSequenceEntity();

        // Enum values for warehouse transaction types
        private List<dynamic> warehouseTransTypes = new List<dynamic>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Populate the dropdown with enum values
            warehouseTransTypes = Enum.GetValues(typeof(EnumWarehouseTransType))
                              .Cast<EnumWarehouseTransType>()
                              .Select(e => new { Text = e.ToString(), Value = e.ToString() }) // Both Text and Value are strings
                              .ToList<dynamic>();

            if (Title.Contains("Detail")) isDisabled = true;

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
                var arg = await _numberSequenceServices.GetByIdAsync(Id.Value);
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
                _model.JournalType = arg.Data.JournalType;
                _model.Prefix = arg.Data.Prefix;
                _model.SequenceLength = arg.Data.SequenceLength;
                _model.CurrentSequenceNo = arg.Data.CurrentSequenceNo;
            }
            #endregion

            StateHasChanged();
        }

        async Task Submit(NumberSequenceEntity arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to Save: {arg.JournalType}?", "Save", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            if (Title.Contains("Create")) // Add new number sequence
            {
                var res = await _numberSequenceServices.InsertAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully created",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/numbersequence", true);
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

            if (Title.Contains("Edit")) // Update existing number sequence
            {
                var res = await _numberSequenceServices.UpdateAsync(arg);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Successfully edited",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/numbersequence", true);
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

        async Task DeleteItemAsync(NumberSequenceEntity numberSequence)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete this: {numberSequence.Prefix}?", "Delete", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _numberSequenceServices.DeleteAsync(numberSequence);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete {numberSequence.Prefix} successfully.",
                        Duration = 5000
                    });

                    _navigation.NavigateTo("/numbersequence", true);
                    StateHasChanged();
                }
                else
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = $"Failed to delete {numberSequence.Prefix}.",
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
                    Detail = $"Failed to delete {numberSequence.Prefix}.",
                    Duration = 5000
                });
            }
        }
    }
}
