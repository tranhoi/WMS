function printQRCode() {
    var printWindow = window.open('', '_blank');
    var qrCodeImage = document.getElementById("qrCodeImage").src;
    printWindow.document.write('<html><head><title>Print QR Code</title></head>');
    printWindow.document.write('<body><img src="' + qrCodeImage + '" /></body></html>');
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
    printWindow.close();
}