import { Fancybox }  from "https://cdn.jsdelivr.net/npm/@fancyapps/ui@5.0/dist/fancybox/fancybox.esm.js";
export function previewImage(url, gallery) {

    if (url == null) return;
    if (isImageUrl(url)) {
        let images = [];
        if (gallery != null) {
            images = gallery.filter(l => isImageUrl(l)).map(x => ({ src: x, caption: x.split("/").pop() }));
        } else {
            images = [{ src: url, caption: url.split("/").pop() }];
        }
        const fancybox = new Fancybox(images);
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