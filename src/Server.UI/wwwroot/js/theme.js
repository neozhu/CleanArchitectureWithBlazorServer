export function setRootFontSize(px) {
    var size = typeof px === 'number' ? px : parseFloat(px);
    if (!size || isNaN(size)) size = 14;
    document.documentElement.style.fontSize = size + 'px';
}


