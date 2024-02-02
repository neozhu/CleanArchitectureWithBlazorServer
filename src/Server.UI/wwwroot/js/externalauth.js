export async function externalLogin(provider, dotNetHelper) {
    if (provider == 'Microsoft') {
        await loginWithMicrosoft(provider, dotNetHelper);
    } else if (provider == 'Google') {
        await loginWithGoogle(provider, dotNetHelper);
    }

}
export const MS_CLIENT_ID = '89c688b8-6933-4a93-8bb4-c2c3513a4d76';
export const MS_CLIENT_SECRET = '770077e1-dbbf-4613-b95c-685f6c5d815c';
export const GOOGLE_CLIENT_ID = '297897098383-mst0sd9qi5f7p9r0l0ralai3unsqrqmv.apps.googleusercontent.com';
 async function loginWithMicrosoft(provider, dotNetHelper) {
    var client = new msal.PublicClientApplication({
        auth: {
            clientId: MS_CLIENT_ID,
            clientSecret: MS_CLIENT_SECRET,
            authority: "https://login.microsoftonline.com/common",
            redirectUri: "https://architecture.blazorserver.com/",
        },
        cache: {
            cacheLocation: "sessionStorage", // This configures where your cache will be stored
            storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
        },
        system: {
            loggerOptions: {
                logLevel: msal.LogLevel.Trace,
                loggerCallback: (level, message, containsPii) => {
                    if (containsPii) {
                        return;
                    }
                    switch (level) {
                        case msal.LogLevel.Error:
                            console.error(message);
                            return;
                        case msal.LogLevel.Info:
                            console.info(message);
                            return;
                        case msal.LogLevel.Verbose:
                            console.debug(message);
                            return;
                        case msal.LogLevel.Warning:
                            console.warn(message);
                            return;
                        default:
                            console.log(message);
                            return;
                    }
                }
            }
        },
        telemetry: {
            application: {
                appName: "Blazor",
                appVersion: "1.0.0"
            }
        }
    });
    var response = await client.loginPopup({ scopes: ["User.Read"] });
    await dotNetHelper.invokeMethodAsync('ConfirmExternal', provider, response.account.username, response.account.name, response.accessToken);
    console.log('login with microsoft success');
    localStorage.setItem('microsoft_client_token', response.account.homeAccountId);
}
 async function loginWithGoogle(provider, dotNetHelper) {
    client = google.accounts.oauth2.initTokenClient({
        client_id: GOOGLE_CLIENT_ID,
        scope: 'https://www.googleapis.com/auth/userinfo.email',
        ux_mode: 'popup',
        callback: async response => {
            const access_token = response.access_token;
            const url = `https://www.googleapis.com/oauth2/v3/userinfo?access_token=${access_token}`;
            const data = await fetch(url).then(response => response.json());
            await dotNetHelper.invokeMethodAsync('ConfirmExternal', provider, data.email, data.name, access_token);
            console.log('login with microsoft success');
            localStorage.setItem('google_client_token', access_token);

        }
    });
    client.requestAccessToken();
}
export async function externalLogout() {
    let client_token = localStorage.getItem('microsoft_client_token');
    if (client_token) {
        const client = new msal.PublicClientApplication({
            auth: {
                clientId: MS_CLIENT_ID,
                clientSecret: MS_CLIENT_SECRET,
                authority: "https://login.microsoftonline.com/common",
                redirectUri: "https://architecture.blazorserver.com/",
            },
            cache: {
                cacheLocation: "sessionStorage", // This configures where your cache will be stored
                storeAuthStateInCookie: false, // Set this to "true" if you are having issues on IE11 or Edge
            },
            system: {
                loggerOptions: {
                    logLevel: msal.LogLevel.Trace,
                    loggerCallback: (level, message, containsPii) => {
                        if (containsPii) {
                            return;
                        }
                        switch (level) {
                            case msal.LogLevel.Error:
                                console.error(message);
                                return;
                            case msal.LogLevel.Info:
                                console.info(message);
                                return;
                            case msal.LogLevel.Verbose:
                                console.debug(message);
                                return;
                            case msal.LogLevel.Warning:
                                console.warn(message);
                                return;
                            default:
                                console.log(message);
                                return;
                        }
                    }
                }
            },
            telemetry: {
                application: {
                    appName: "Blazor",
                    appVersion: "1.0.0"
                }
            }
        });
        const logoutRequest = {
            account: client.getAccountByHomeId(client_token)
        };

        await client.logoutPopup(logoutRequest);
        localStorage.removeItem('microsoft_client_token');
    } else {
        client_token = localStorage.getItem('google_client_token');
        if (client_token) {
            google.accounts.oauth2.revoke(client_token);
            localStorage.removeItem('google_client_token');
        }
    }
}