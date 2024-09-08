import {Fancybox} from "https://cdn.jsdelivr.net/npm/@fancyapps/ui@5.0.36/dist/fancybox/fancybox.esm.js";

export function previewImage(url, gallery) {
    console.log(url);
    if (url == null) return;
    const fileName = getFileName(url);
    if (isImageUrl(url)) {
        let images = [];
        if (gallery != null && gallery.length > 0) {
            images = gallery.filter(l => isImageUrl(l)).map(x => ({src: x, caption: x.split("/").pop()}));
        } else {
            images = [{src: url, caption: url.split("/").pop()}];
        }
        const fancybox = new Fancybox(images);
    } else if (isPDF(url)) {
        const fancybox = new Fancybox([{ src: url, type: 'pdf', caption: url }]);
    } else {
        const anchorElement = document.createElement('a');
        anchorElement.href = url;
        anchorElement.download = fileName ?? '';
        anchorElement.click();
        anchorElement.remove();
    }
}

function isImageUrl(url) {
    const imageExtensions = /\.(gif|jpe?g|tiff?|png|webp|bmp)$/i;
    return imageExtensions.test(url);
}
function isPDF(url) {
    return url.toLowerCase().endsWith('.pdf');
}
function getFileName(url) {
    return url.split('/').pop();
}