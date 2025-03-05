import OpenSeadragon from 'https://cdn.jsdelivr.net/npm/openseadragon@5.0/+esm'

export function showOpenSeadragon(element, url) {
    var viewer = OpenSeadragon({
        id: element,
        showNavigator: false,
        showZoomControl: false,
        showHomeControl: false,
        showFullPageControl: false,
        showSequenceControl: false,
        maxZoomPixelRatio: 3,
        minZoomImageRatio: 0.3,
        tileSources: {
            xmlns: "http://schemas.microsoft.com/deepzoom/2008",
            buildPyramid: false,
            crossOriginPolicy: 'Anonymous',
            type: 'image',
            url,
        }
    });
    //console.log(viewer)
}