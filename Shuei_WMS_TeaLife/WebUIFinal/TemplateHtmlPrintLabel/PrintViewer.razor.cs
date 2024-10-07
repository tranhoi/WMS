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

            if (LabelPrintModel != null)
            {
                StateHasChanged();
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Đợi một giây để đảm bảo nội dung đã được render
                await Task.Delay(2000);
                // Gọi hàm in
                _ = _jsRuntime.InvokeVoidAsync("printLabel");
            }
        }
    }
}
