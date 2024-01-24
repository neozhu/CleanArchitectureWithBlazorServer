export function loadscript() {
    const script = document.createElement('script');
    script.setAttribute(
        'src',
        'https://unpkg.com/fabric@5.2.1/dist/fabric.min.js',
    );
    script.onload = function handleScriptLoaded() {
        console.log('fabric has loaded');
        var x = document.createElement("canvas");
        document.body.appendChild(x);
        console.log(new fabric.Canvas('canvas'));
    };
    document.head.appendChild(script);
}

export function destroyscript() {
    var tags = document.head.getElementsByTagName('script'), index;
    for (index = tags.length - 1; index >= 0; index--) {
        if (tags[index].src == 'https://unpkg.com/fabric@5.2.1/dist/fabric.min.js') {
            tags[index].parentNode.removeChild(tags[index]);
        }
    }

}