import { Fancybox } from "https://cdn.jsdelivr.net/npm/@fancyapps/ui@6.1.7/dist/fancybox/index.js";

const CSS_URL = 'https://unpkg.com/@fancyapps/ui@6.1.7/dist/fancybox/fancybox.css';

// Use a variable to store the Promise, ensuring CSS is loaded only once and subsequent calls can await directly
let cssLoadingPromise = null;

/**
 * Responsible for loading CSS and returning a Promise
 * Solves the issue of repeated loading and waiting for loading completion
 */
function loadFancyboxStyles() {
    if (cssLoadingPromise) {
        return cssLoadingPromise;
    }

    cssLoadingPromise = new Promise((resolve, reject) => {
        // Check if the CSS already exists in the page (prevents duplicate imports elsewhere)
        if (document.querySelector(`link[href="${CSS_URL}"]`)) {
            resolve();
            return;
        }

        const link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = CSS_URL;

        // Important: Bind the onload event to ensure the styles are fully loaded before resolving
        link.onload = () => resolve();
        link.onerror = () => {
            cssLoadingPromise = null; // Reset on failure to allow retry
            reject(new Error(`Failed to load Fancybox CSS from ${CSS_URL}`));
        };

        document.head.appendChild(link);
    });

    return cssLoadingPromise;
}

// Export function changed to async because it needs to wait for CSS to load
export async function previewImage(url, gallery) {
    if (!url) return;

    const fileName = getFileName(url);

    try {
        // 1. Ensure CSS is ready before showing the preview
        await loadFancyboxStyles();

        // 2. Image preview logic
        if (isImageUrl(url)) {
            const slides = (gallery && gallery.length > 0 ? gallery : [url])
                .filter(isImageUrl)
                .map(x => ({
                    src: x,
                    thumbSrc: x,
                    caption: x.split("/").pop()
                }));

            // Start preview from the currently clicked image (assumes gallery contains url)
            const startIndex = slides.findIndex(s => s.src === url);

            Fancybox.show(slides, {
                startIndex: startIndex > -1 ? startIndex : 0, // Optimization: open from current image
                animated: true,
                dragToClose: true,
                Thumbs: {
                    autoStart: slides.length > 1
                }
            });

            return;
        }

        // 3. PDF preview logic
        if (isPDF(url)) {
            Fancybox.show(
                [{
                    src: url,
                    type: "pdf",
                    caption: fileName
                }],
                {
                    dragToClose: false
                }
            );
            return;
        }

        // 4. Fallback: download file
        downloadFile(url, fileName);

    } catch (error) {
        console.error("Preview failed:", error);
        // If CSS loading fails, still attempt to download as a fallback
        downloadFile(url, fileName);
    }
}

// --- Helper Functions ---

function downloadFile(url, fileName) {
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName ?? "";
    document.body.appendChild(a); // Firefox requires appending to body to trigger click
    a.click();
    a.remove();
}

function isImageUrl(url) {
    // Add handling to ignore query parameters for better robustness
    const cleanUrl = url.split('?')[0];
    return /\.(gif|jpe?g|tiff?|png|webp|bmp|svg)$/i.test(cleanUrl);
}

function isPDF(url) {
    const cleanUrl = url.split('?')[0];
    return cleanUrl.toLowerCase().endsWith(".pdf");
}

function getFileName(url) {
    // Handle URL decoding (e.g., for filenames with non-ASCII characters)
    try {
        return decodeURIComponent(url.split("/").pop().split('?')[0]);
    } catch (e) {
        return url.split("/").pop();
    }
}