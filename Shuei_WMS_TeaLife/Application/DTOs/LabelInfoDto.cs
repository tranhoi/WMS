using Magicodes.ExporterAndImporter.Core;

namespace Application.DTOs
{
    [Exporter(Name = "REPORT 1")]
    public class LabelInfoDto
    {
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Base64 code.
        /// </summary>
        public string QrValue { get; set; }=string.Empty;
        public string Title1 { get; set; } = string.Empty;
        public string Title2 { get; set; } = string.Empty;
        public string Title3 { get; set; } = string.Empty;
        public string Title4 { get; set; } = string.Empty;
        public string Content1 { get; set; } = string.Empty;
        public string Content2 { get; set; } = string.Empty;
        public string Content3 { get; set; } = string.Empty;
        public string Content4 { get; set; } = string.Empty;
    }
}
