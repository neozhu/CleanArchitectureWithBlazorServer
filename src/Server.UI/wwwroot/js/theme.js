export function setRootFontSize(px) {
    let size = typeof px === 'number' ? px : parseFloat(px);
    if (!size || isNaN(size)) size = 15;

    const root = document.documentElement;

    root.style.fontSize = `${size}px`;
    // const paddingX = size; 
    // const paddingY = Math.round(size * 0.5); 

    // root.style.setProperty('--cell-padding', `${paddingX}px`); 
    // root.style.setProperty('--cell-padding-x', `${paddingX}px`); 
    // root.style.setProperty('--cell-padding-y', `${paddingY}px`); 

    // const navPaddingX = size + 2; 
    // const navPaddingY = Math.round(size * 0.75); 

    // root.style.setProperty('--nav-padding-x', `${navPaddingX}px`);
    // root.style.setProperty('--nav-padding-y', `${navPaddingY}px`);
}


