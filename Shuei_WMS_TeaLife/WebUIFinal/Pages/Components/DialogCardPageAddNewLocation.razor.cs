using Application.DTOs;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Domain.Enums;
using Application.Extentions;
using Domain.Entity.authp.Commons;
using Domain.Entity.Commons;
using Domain.Entity.WMS;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Radzen;
using Radzen.Blazor;
using System.Security.Cryptography;
using WebUIFinal.Pages.Device;
using WebUIFinal.TemplateHtmlPrintLabel;

namespace WebUIFinal.Pages.Components
{
    public partial class DialogCardPageAddNewLocation
    {
        [Parameter] public string Title { get; set; } = string.Empty;

        [Parameter] public Location _model { get; set; } = new Location();

        EnumStatus _selectStatus;
        bool _visibleBtnSubmit = true, _disable = false;
        string _id = string.Empty;

        List<Bin> _dataGrid = [];
        IList<Bin> _selectedDataBinList = [];
        RadzenDataGrid<Bin>? _profileGrid;
        bool allowRowSelectOnRowClick = true;
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 50, 100 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";

        List<Bin> _listRemoveBin = new List<Bin>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizer["DisplayPage"] + " {0} " + _localizer["Of"] + " {1} <b>(" + _localizer["Total"] + " {2} " + _localizer["Records"] + ")</b>";

            await RefreshDataAsync();

            StateHasChanged();
        }

        async Task RefreshDataAsync()
        {
            try
            {
                _selectStatus = EnumStatus.Activated;

                if (Title.Contains("|"))
                {
                    if (Title.Contains($"{_localizer["Detail.View"]}"))
                    {
                        _visibleBtnSubmit = false;
                        _disable = true;
                    }

                    var arr = Title.Split('|');
                    Title = arr[0];
                    _id = arr[1];

                    var res = await _locationServices.GetByIdAsync(Guid.Parse(_id));
                    var resMessage = res.Messages.FirstOrDefault();
                    if (!res.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = resMessage,
                            Duration = 5000
                        });
                        return;
                    }

                    _model = res.Data;

                    //get bin information
                    var resBin = await _binServices.GetByLocationId(_model.Id);
                    resMessage = res.Messages.FirstOrDefault();
                    if (!res.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = resMessage,
                            Duration = 5000
                        });
                        return;
                    }

                    _dataGrid = resBin.Data;

                    _selectStatus = _model.Status;
                }
                else
                {
                    _model.Id = Guid.NewGuid();
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

        async void Submit(Location arg)
        {
            try
            {
                var l = _localizer["Detail.Create"];

                if (Title.Contains(_localizer["Detail.Create"]))
                {
                    var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Create"]} {_localizer["Location"]}: {arg.LocationName}?", $"{_localizer["Detail.Create"]} {_localizer["Location"]}", new ConfirmOptions()
                    {
                        OkButtonText = "Yes",
                        CancelButtonText = "No",
                        AutoFocusFirstElement = true,
                    });

                    if (confirm == null || confirm == false) return;
                }
                else
                {
                    var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Update"]} {_localizer["Location"]}: {arg.LocationName}?", $"{_localizer["Update"]} {_localizer["Location"]}", new ConfirmOptions()
                    {
                        OkButtonText = "Yes",
                        CancelButtonText = "No",
                        AutoFocusFirstElement = true,
                    });

                    if (confirm == null || confirm == false) return;
                }

                _model.Status = _selectStatus;

                string resMess = string.Empty;
                if (Title.Contains(_localizer["Detail.Create"]))
                {
                    //_model.Id = Guid.NewGuid();
                    var response = await _locationServices.InsertAsync(_model);
                    resMess = response.Messages.FirstOrDefault();
                    if (!response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = resMess,
                            Duration = 5000
                        });
                        return;
                    }
                }
                else if (Title.Contains(_localizer["Detail.Edit"]))
                {
                    var response = await _locationServices.UpdateAsync(_model);

                    if (!response.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = response.Messages.FirstOrDefault(),
                            Duration = 5000
                        });

                        return;
                    }

                    resMess = response.Messages.FirstOrDefault();
                }

                //BIN
                if (_listRemoveBin.Count > 0)
                {
                    var responseDeleteNin = await _binServices.DeleteRangeAsync(_listRemoveBin);
                    resMess = responseDeleteNin.Messages.FirstOrDefault();

                    if (!responseDeleteNin.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = resMess,
                            Duration = 5000
                        });

                        return;
                    }
                }

                if (_dataGrid.Count > 0)
                {
                    foreach (var item in _dataGrid)
                    {
                        item.LocationCD = _model.LocationCD;
                        item.LocationName = _model.LocationName;
                    }

                    var responseBin = await _binServices.AddOrUpdateAsync(_dataGrid);
                    resMess = responseBin.Messages.FirstOrDefault();

                    if (!responseBin.Succeeded)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = resMess,
                            Duration = 5000
                        });

                        return;
                    }
                }

                _notificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = resMess,
                    Duration = 5000
                });

                _dialogService.Close("Success");
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

        async Task PrintLable()
        {
            var dataPrint = await _binServices.GetLabelByLocationIdAsync(_model.Id);
            var res = await _dialogService.OpenAsync<PrintViewer>(string.Empty,
                    new Dictionary<string, object>() { { "LabelPrintModel", dataPrint }, { "Title", $"{_localizer["PrintLabelFor"]} {_localizer["Bin"]}" } },
                    new DialogOptions()
                    {
                        Width = "1000px",
                        Height = "1000px",
                        Resizable = true,
                        Draggable = true,
                        ShowClose = false,
                        CloseDialogOnOverlayClick = true
                    });

            //if (res == "Success")
            //{
            //    RefreshDataAsync();
            //}
        }

        async Task AddBin()
        {
            try
            {
                Bin binInfor = new Bin()
                {
                    LocationId = _model.Id
                };

                var res = await _dialogService.OpenAsync<DialogCardPageAddNewBin>($"{_localizer["Detail.Create"]} {_localizer["Bin"]}",
                        new Dictionary<string, object>() { { "_model", binInfor }, { "VisibleBtnSubmit", true } },
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
                    var selectResult = (Bin)res;

                    var returnModel = _dataGrid.FirstOrDefault(x => x.BinCode == selectResult.BinCode);

                    if (returnModel != null)
                    {
                        _notificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = $"{selectResult.BinCode} has been exist.",
                            Duration = 5000
                        });
                        return;
                    }

                    _dataGrid.Add(selectResult);

                    await _profileGrid.RefreshDataAsync();
                }
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

        async Task DeleteItemAsync(Bin model)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"{_localizer["Confirmation.Delete"]} {_localizer["Bin"]}: {model.BinCode}?", $"{_localizer["Delete"]} {_localizer["Bin"]}", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                _dataGrid.Remove(model);
                _listRemoveBin.Add(model);

                await _profileGrid.RefreshDataAsync();
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

        async Task ViewItemAsync(Bin model)
        {
            try
            {
                var res = await _dialogService.OpenAsync<DialogCardPageAddNewBin>($"{_localizer["Detail.View"]} {_localizer["Bin"]}",
                   new Dictionary<string, object>() { { "_model", model }, { "VisibleBtnSubmit", false } },
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
        async Task EditItemAsync(Bin model)
        {
            try
            {
                var res = await _dialogService.OpenAsync<DialogCardPageAddNewBin>($"{_localizer["Detail.Edit"]} {_localizer["Bin"]}",
                   new Dictionary<string, object>() { { "_model", model }, { "VisibleBtnSubmit", true } },
                   new DialogOptions()
                   {
                       Width = "1000",
                       Height = "400",
                       Resizable = true,
                       Draggable = true,
                       CloseDialogOnOverlayClick = true
                   });

                if (res != null)
                {
                    await _profileGrid.RefreshDataAsync();
                }
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
    }
}
