using Domain.Enums;
using Domain.Entity.WMS;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace WebUIFinal.Pages.UnitPage
{
    public partial class UnitDetail
    {
        [Parameter] public string Title { get; set; }

        private bool isDisabled = false;
        private Unit _model = new Unit();
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
                    if (Title.Contains("View"))
                    {
                        _visibleBtnSubmit = false;
                        _disable = true;
                    }

                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var res = await _unitsService.GetByIdAsync(int.Parse(_id));

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


        async void Submit(Unit arg)
        {
            var confirm = await _dialogService.Confirm($"Do you want to Save: {arg.UnitName}?", "Unit", new ConfirmOptions()
            {
                OkButtonText = "Yes",
                CancelButtonText = "No",
                AutoFocusFirstElement = true,
            });

            if (confirm == null || confirm == false) return;

            arg.Status = _selectStatus;

            if (Title.Contains(_localizer["Detail.Create"]))//Add
            {
                var res = await _unitsService.InsertAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully created Unit",
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
            else if (Title.Contains(_localizerCommon["Detail.Edit"]))//update
            {
                var res = await _unitsService.UpdateAsync(_model);
                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Sucessfully edited Unit",
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

        async Task DeleteItemAsync(Unit Unit)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete Unit: {Unit.UnitName}?", "Delete Unit", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _unitsService.DeleteAsync(Unit);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Delete Unit {Unit.UnitName} successfully.",
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
                    Detail = $"Failed to delete Unit {Unit.UnitName}.",
                    Duration = 5000
                });

                return;
            }
        }
    }
}
