using Magicodes.ExporterAndImporter.Core;

namespace Application.DTOs
{
    [Exporter(Name = "USER INFO")]
    public class ReceiptInfo
    {
        public string Base64QR { get; set; }
        public string Title { get; set; }
        public string MyProperty { get; set; }
    }
}
