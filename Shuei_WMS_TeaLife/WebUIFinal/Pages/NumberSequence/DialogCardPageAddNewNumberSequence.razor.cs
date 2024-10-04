using Application.Enums;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebUIFinal.Core;
using NumberSequenceEntity = Domain.Entity.WMS.NumberSequences;

namespace WebUIFinal.Pages.NumberSequence
{
    public partial class DialogCardPageAddNewNumberSequence
    {
        [Parameter] public string Title { get; set; }
        public Guid? Id { get; set; }

        private bool isDisabled = false;
        private NumberSequenceEntity _model = new NumberSequenceEntity();
        private List<string> _status = new List<string>();
        private Status selectStatus;

        // Enum values for warehouse transaction types
        private List<dynamic> warehouseTransTypes = new List<dynamic>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Populate the dropdown with enum values
            warehouseTransTypes = Enum.GetValues(typeof(EnumWarehouseTransType))
                              .Cast<EnumWarehouseTransType>()
                              .Select(e => new { Text = e.ToString(), Value = e.ToString() })
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
                var resultNumberSequence = await _numberSequenceServices.GetByIdAsync(Id.Value);
                if (resultNumberSequence == null)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Result number sequence null",
                        Duration = 1000
                    });

                    return;
                }

                _model.Id = resultNumberSequence.Data.Id;
                _model.JournalType = resultNumberSequence.Data.JournalType;
                _model.Prefix = resultNumberSequence.Data.Prefix;
                _model.SequenceLength = resultNumberSequence.Data.SequenceLength;
                _model.CurrentSequenceNo = resultNumberSequence.Data.CurrentSequenceNo;

                if (!string.IsNullOrEmpty(resultNumberSequence.Data.Status))
                {
                    selectStatus = CommonHelpers.ParseEnum<Status>(resultNumberSequence.Data.Status);
                }
            }
            #endregion

            StateHasChanged();
        }

        async void Submit(NumberSequenceEntity arg)
        {
            // Existing submit logic
        }

        async Task DeleteItemAsync(NumberSequenceEntity numberSequence)
        {
            // Existing delete logic
        }
    }
}
