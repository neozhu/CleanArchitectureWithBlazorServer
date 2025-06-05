// Import QRCode library (you may need to include this via a <script> tag in your index.html or _Host.cshtml)
// For this example, I'm assuming you're using the qrcode.js library

// Export the function to make it available for JSInterop
export function generateQRCode(url) {
    // Create a container for the QR code
    const container = document.createElement('div');

    // Generate a unique ID for the container
    const uniqueId = 'qrcode_' + new Date().getTime();
    container.id = uniqueId;

    // Temporarily add to document to generate QR code
    document.body.appendChild(container);

    // Generate QR code (using a hypothetical QRCode library)
    // You'll need to include a QR code library in your project
    const qrcode = new QRCode(container, {
        text: url,
        width: 128,
        height: 128,
        colorDark: "#000000",
        colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });

    // Convert to data URL (base64)
    const canvas = container.querySelector('canvas');
    const dataUrl = canvas ? canvas.toDataURL('image/png') : '';

    // Clean up
    document.body.removeChild(container);

    // Return the QR code as a data URL
    return dataUrl;
}