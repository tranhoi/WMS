using Azure;
using Domain.Entity.WMS;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using QRCoder.Core;
using WebUIFinal.Core.Dto;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PrintViewer
    {
        [Parameter]
        public string ApiInfo { get; set; }

        private string _id { get; set; }
        private string _api { get; set; }
        private string _hrefReturn { get; set; }

        private List<PrintLabelInfoDto> _labelInfo = new List<PrintLabelInfoDto>();

        private string inputText = string.Empty;
        private string qrCodeBase64 = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var arr=ApiInfo.Split('|');
            _api= arr[0];
            _id= arr[1];
            _hrefReturn = arr[2];

            if (_api == "Bin")
            {
                List<Bin> binInfo = new List<Bin>();
                var responseBin = await _binServices.GetAllAsync();
                if (responseBin.Succeeded) binInfo = responseBin.Data;

                foreach (var bin in binInfo)
                {
                    List<LineInfo> li = new List<LineInfo>();
                    for (int j = 1; j <= 4; j++)
                    {
                        var lineInfo = j <= 2 ? true : false;
                        li.Add(new LineInfo()
                        {
                            IsVisible = lineInfo,
                            Title = j == 1 ? $"Location Id:":"Bin Code:",
                            Content = j==1?bin.LocationId.ToString():bin.BinCode
                        });
                    }

                    _labelInfo.Add(new PrintLabelInfoDto()
                    {
                        QrValue = GlobalVariable.GenerateQRCode($"{bin.Id}|{bin.LocationId}|{bin.BinCode}|{bin.Remarks}"),
                        Lines = li
                    });
                }
            }
            else if (_api == "Product")
            {
                List<Bin> binInfo = new List<Bin>();
                var responseBin = await _binServices.GetAllAsync();
                if (responseBin.Succeeded) binInfo = responseBin.Data;

                foreach (var bin in binInfo)
                {
                    List<LineInfo> li = new List<LineInfo>();
                    for (int j = 1; j <= 4; j++)
                    {
                        var v = j <= 1 ? false : true;
                        li.Add(new LineInfo()
                        {
                            IsVisible = v,
                            Title = $"Line {j}",
                            Content = $"Content {j}"
                        });
                    }

                    _labelInfo.Add(new PrintLabelInfoDto()
                    {
                        //QrValue = GlobalVariable.GenerateQRCode($"Index:{i}|tesstCode|jshshshsh"),
                        Lines = li
                    });
                }
            }
        }

        private async Task PrintLabel()
        {
            await _jsRuntime.InvokeVoidAsync("printLabel");
        }
    }
}
