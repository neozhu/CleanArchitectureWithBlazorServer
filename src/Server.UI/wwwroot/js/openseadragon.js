let _osdPromise;

function loadScriptOnce(src) {
    return new Promise((resolve, reject) => {
        const existing = document.querySelector(`script[data-src="${src}"]`);
        if (existing) {
            if (existing.getAttribute("data-loaded") === "1") return resolve();
            existing.addEventListener("load", () => resolve(), { once: true });
            existing.addEventListener("error", () => reject(new Error(`Failed to load ${src}`)), { once: true });
            return;
        }

        const s = document.createElement("script");
        s.src = src;
        s.async = true;
        s.defer = true;
        s.setAttribute("data-src", src);
        s.onload = () => {
            s.setAttribute("data-loaded", "1");
            resolve();
        };
        s.onerror = () => reject(new Error(`Failed to load ${src}`));
        document.head.appendChild(s);
    });
}

async function ensureOpenSeadragon() {
    if (!_osdPromise) {
        _osdPromise = (async () => {
            await loadScriptOnce("https://unpkg.com/openseadragon@5.0.1/build/openseadragon/openseadragon.min.js");
            const osd = window.OpenSeadragon;
            if (!osd) throw new Error("OpenSeadragon not found on window after loading UMD bundle.");
            return osd;
        })();
    }
    return _osdPromise;
}

export async function showOpenSeadragon(elementId, url) {
    const OpenSeadragon = await ensureOpenSeadragon();

    // 防御：确保容器存在
    const el = typeof elementId === "string" ? document.getElementById(elementId) : null;
    if (!el) return;

    // 若重复打开，先销毁旧实例（避免泄漏/多次绑定事件）
    if (el._osdViewer) {
        try { el._osdViewer.destroy(); } catch { /* ignore */ }
        el._osdViewer = null;
    }

    const viewer = OpenSeadragon({
        id: elementId,
        showNavigator: false,
        showZoomControl: false,
        showHomeControl: false,
        showFullPageControl: false,
        showSequenceControl: false,
        maxZoomPixelRatio: 3,
        minZoomImageRatio: 0.3,
        // 重要：如果你只展示普通图片，用 type:'image' OK
        tileSources: {
            type: "image",
            url,
            crossOriginPolicy: "Anonymous",
        }
    });

    el._osdViewer = viewer;
    return viewer;
}
