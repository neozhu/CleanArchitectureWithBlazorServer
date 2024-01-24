export function clearInput(inputId) {
    setTimeout(function () {
        var input = document.querySelector("#" + inputId);
        if (input) {
            input.value = "";
        }
    }, 30);
}