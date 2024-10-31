using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QRCoder.Core;
using System.Text.Json;
using WebUIFinal.TemplateHtmlPrintLabel;
using PutAwayModel = FBT.ShareModels.WMS.WarehousePutAway;


namespace WebUIFinal.Pages.Putaway
{
    public partial class PutawayList
    {
        IEnumerable<int> _pageSizeOptions = new int[] { 5, 10, 20, 30, 100, 200 };
        bool _showPagerSummary = true;
        string _pagingSummaryFormat = "Displaying page {0} of {1} <b>(total {2} records)</b>";
        DateTime? value;
        List<PutAwayModel> _dataGrid = new();
        RadzenDataGrid<PutAwayModel>? _putAwayGrid;
        private List<PutAwayModel> _filteredModel = new List<PutAwayModel>(); // Added this line
        public bool IsDeleted { get; set; }
        bool allowRowSelectOnRowClick = true;
        IEnumerable<PutAwayModel> PutAway = [];
        IList<PutAwayModel> selectedPutAway = [];

        private string inputText = string.Empty;
        private string qrCodeBase64 = string.Empty;
        List<LocationDisplayDto> locations = new();
        string _id = string.Empty;

        bool _visibaleProgressBar = false;
        [Inject] private ILocalStorageService _localStorage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            _pagingSummaryFormat = _localizerCommon["DisplayPage"] + " {0} " + _localizerCommon["Of"] + " {1} <b>(" + _localizerCommon["Total"] + " {2} " + _localizerCommon["Records"] + ")</b>";

            await RefreshDataAsync();
            await GetLocationssAsync();

            _filteredPutAwayItems = new List<PutAwayModel>(_dataGrid);
        }

        private async Task GetLocationssAsync()
        {
            var data = await _locationServices.GetAllAsync();
            if (data.Succeeded) locations.AddRange(data.Data.Select(_ => new LocationDisplayDto { Id = _.Id.ToString(), LocationName = _.LocationName }));
        }

        // void NavigateDetailPage(string PutAwayNo) => _navigation.NavigateTo($"/putawaydetails/{"Detail.View"}|"+PutAwayNo);
        void EditItemAsync(string PutAwayNo) => _navigation.NavigateTo($"/putawaydetails/|" + PutAwayNo);
        private void GenerateQRCode()
        {
            inputText = "NGUYEN DINH CONG|COng123@456";
            if (string.IsNullOrEmpty(inputText))
                return;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                qrCodeBase64 = $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }

        // Method to print the QR code
        private async Task PrintQRCode()
        {
            await _jsRuntime.InvokeVoidAsync("printQRCode");
        }

        private async Task PrintSelectedLabels()
        {
            if (selectedPutAway == null || !selectedPutAway.Any())
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Shelvingdetails",
                    Detail = "Please select at least one item to print labels.",
                    Duration = 4000
                });
                return;
            }
            _visibaleProgressBar = true;

            List<LabelInfoDto> labelsToPrint = new List<LabelInfoDto>();

            foreach (var item in selectedPutAway)
            {
                var putawayPrint = await _warehousePutAwayServices.GetPutAwayAsync(item.PutAwayNo);
                if (putawayPrint.Succeeded)
                {
                    foreach (var line in putawayPrint.Data.WarehousePutAwayLines)
                    {
                        if (line.ProductJanCodes.Count == 0)
                        {
                            string qrCodeContent = $"商品コード:{line.ProductCode}\nJANコード:{"N/A"}\nLOT:{line.LotNo}\n賞味期限:{line.ExpirationDate:yyyy/MM/dd}";
                            labelsToPrint.Add(new LabelInfoDto()
                            {
                                QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
                                Title1 = "商品コード",
                                Content1 = line.ProductCode,
                                Title2 = "JANコード",
                                Content2 = "N/A",
                                Title3 = "LOT",
                                Content3 = line.LotNo,
                                Title4 = "賞味期限",
                                Content4 = line.ExpirationDate?.ToString("yyyy/MM/dd") ?? "N/A"
                            });
                        }
                        else
                        {
                            line.ProductJanCodes.ForEach(t =>
                            {
                                string qrCodeContent = $"商品コード:{line.ProductCode}\nJANコード:{t}\nLOT:{line.LotNo}\n賞味期限:{line.ExpirationDate:yyyy/MM/dd}";
                                labelsToPrint.Add(new LabelInfoDto()
                                {
                                    QrValue = GlobalVariable.GenerateQRCode(qrCodeContent),
                                    Title1 = "商品コード",
                                    Content1 = line.ProductCode,
                                    Title2 = "JANコード",
                                    Content2 = t,
                                    Title3 = "LOT",
                                    Content3 = line.LotNo,
                                    Title4 = "賞味期限",
                                    Content4 = line.ExpirationDate?.ToString("yyyy/MM/dd") ?? "N/A"
                                });
                            });
                        }
                    }
                }
            }

            _visibaleProgressBar = false;

            // Lưu labelsToPrint vào LocalStorage
            await _localStorage.SetItemAsync("labelData", labelsToPrint);

            // Điều hướng mà không cần truyền dữ liệu qua URL
            _navigation.NavigateTo("/printlabel");
        }
        async Task DeleteItemAsync(PutAwayModel putAway)
        {
            try
            {
                var confirm = await _dialogService.Confirm($"Are you sure you want to delete put away: {putAway.PutAwayNo}?", "Delete put away", new ConfirmOptions()
                {
                    OkButtonText = "Yes",
                    CancelButtonText = "No",
                    AutoFocusFirstElement = true,
                });

                if (confirm == null || confirm == false) return;

                var res = await _warehousePutAwayServices.DeleteAsync(putAway);

                if (res.Succeeded)
                {
                    _notificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = res.Messages.ToString(),
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
                        Detail = res.Messages.ToString(),
                        Duration = 5000
                    });
                }

                await RefreshDataAsync();
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

                return;
            }
        }




        async Task RefreshDataAsync()
        {
            try
            {
                var res = await _warehousePutAwayServices.GetAllAsync();

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

                _dataGrid = res.Data.ToList();
                // Join the location data with the PutAwayModel items

                _filteredPutAwayItems = _dataGrid;

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
                return;
            }
        }
    }
}
