using Application.DTOs;
using Azure;
using Domain.Entity.WMS;
using Domain.Entity.WMS.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using QRCoder.Core;
using WebUIFinal.Core.Dto;

namespace WebUIFinal.TemplateHtmlPrintLabel
{
    public partial class PrintViewer
    {
        [Parameter] public List<LabelInfoDto> LabelPrintModel { get; set; }
        [Parameter] public string Title { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (LabelPrintModel != null && LabelPrintModel.Count == 1)
            {
                var s = LabelPrintModel.FirstOrDefault();

                LabelPrintModel.Add(new Application.DTOs.LabelInfoDto()
                {
                    QrValue = s.QrValue,
                    Title1 = s.Title1,
                    Content1 = s.Content1,
                    Title2 = s.Title2,
                    Content2 = s.Content2,
                });

                StateHasChanged();
            }
        }

        private async Task PrintLabel()
        {
            await _jsRuntime.InvokeVoidAsync("printLabel");
        }
    }
}
