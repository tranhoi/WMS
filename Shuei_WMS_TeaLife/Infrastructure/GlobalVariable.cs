using Microsoft.AspNetCore.Components.Forms;
using QRCoder.Core;

namespace Infrastructure
{
    public static class GlobalVariable
    {
        /// <summary>
        /// Method to generate QR code base64.
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public static string GenerateQRCode(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
                return null;

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(inputText, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(20);

                return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
            }
        }
    }
}
