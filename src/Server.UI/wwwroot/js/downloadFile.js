export function downloadFile(fileName, byteBase64, contentType) {
    // Create a data URL using the MIME type and the Base64 encoded string
    const url = `data:${contentType};base64,${byteBase64}`;
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;

    // Append the link to the document, trigger a click, and remove the link afterward
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
