// wwwroot/js/auth.js
export function postLogin({ userName, password, rememberMe, returnUrl }) {
    const f = document.createElement("form");
    f.method = "POST";
    f.action = "/pages/authentication/login";
    f.style.display = "none";
    const add = (n, v) => {
        const i = document.createElement("input");
        i.type = "hidden";
        i.name = n;
        i.value = v ?? "";
        f.appendChild(i);
    };
    add("userName", userName);
    add("password", password);
    add("rememberMe", rememberMe);
    if (returnUrl) add("returnUrl", returnUrl);
    const xsrf = document.querySelector('meta[name="xsrf-token"]')?.content;
    if (xsrf) add("__RequestVerificationToken", xsrf);
    document.body.appendChild(f);
    f.submit();
}
