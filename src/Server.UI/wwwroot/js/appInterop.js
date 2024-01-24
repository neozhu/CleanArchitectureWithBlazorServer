import {dotnet} from './notnet.js'

import OpenSeadragon from 'https://cdnjs.cloudflare.com/ajax/libs/openseadragon/3.1.0/openseadragon.min.js';


const {
    setModelImports,
    getAssemblyExports,
    getConfig
} = await dotnet.withDiagnosticTracing(false).withApplicationArgumentsFromQuery().create();

setModelImports('appInterop.js', {
    window: {
        showOpenSeadragon: (element, url) => {
            console.log('showOpenSeadragon');
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
                    type: 'image',
                    url,
                }
            });
            console.log(viewer);
        }
    }
});


const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);


await dotnet.run();