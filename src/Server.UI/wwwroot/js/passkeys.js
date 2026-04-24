const browserSupportsPasskeys =
    typeof navigator.credentials !== 'undefined' &&
    typeof window.PublicKeyCredential !== 'undefined' &&
    typeof window.PublicKeyCredential.parseCreationOptionsFromJSON === 'function' &&
    typeof window.PublicKeyCredential.parseRequestOptionsFromJSON === 'function';

// Module-level variable to store abort controller
let abortController = null;

/**
 * Converts ArrayBuffer to Base64URL string
 */
function arrayBufferToBase64Url(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.byteLength; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    const base64 = btoa(binary);
    return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}

/**
 * Serializes PublicKeyCredential to JSON format for server (for authentication)
 */
function serializeAuthenticationCredential(credential) {
    const response = {
        id: credential.id,
        rawId: arrayBufferToBase64Url(credential.rawId),
        type: credential.type,
        response: {
            clientDataJSON: arrayBufferToBase64Url(credential.response.clientDataJSON),
            authenticatorData: arrayBufferToBase64Url(credential.response.authenticatorData),
            signature: arrayBufferToBase64Url(credential.response.signature),
            userHandle: credential.response.userHandle ? arrayBufferToBase64Url(credential.response.userHandle) : null
        },
        // Add clientExtensionResults (required by ASP.NET Core Identity)
        clientExtensionResults: credential.getClientExtensionResults ? credential.getClientExtensionResults() : {}
    };
    
    return response;
}

/**
 * Serializes PublicKeyCredential to JSON format for server
 */
function serializeCredential(credential) {
    const response = {
        id: credential.id,
        rawId: arrayBufferToBase64Url(credential.rawId),
        type: credential.type,
        response: {
            clientDataJSON: arrayBufferToBase64Url(credential.response.clientDataJSON),
            attestationObject: arrayBufferToBase64Url(credential.response.attestationObject),
            authenticatorData: credential.response.authenticatorData ? arrayBufferToBase64Url(credential.response.authenticatorData) : null,
            publicKey: credential.response.publicKey ? arrayBufferToBase64Url(credential.response.publicKey) : null,
            publicKeyAlgorithm: credential.response.publicKeyAlgorithm || null,
            transports: credential.response.getTransports ? credential.response.getTransports() : []
        },
        // Add clientExtensionResults (required by ASP.NET Core Identity)
        clientExtensionResults: credential.getClientExtensionResults ? credential.getClientExtensionResults() : {}
    };
    
    return response;
}

/**
 * Creates a Passkey credential
 * Calls the backend to get creation options, then prompts Windows Security for user to create a passkey
 * Returns the serialized credential in JSON format
 */
export async function createPasskeyCredential() {
    if (!browserSupportsPasskeys) {
        throw new Error('Browser does not support passkeys');
    }

    try {
        // Get XSRF token for security protection
        const xsrf = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        // Call backend to get passkey creation options
        const response = await fetch('/pages/authentication/PasskeyCreationOptions', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': xsrf || ''
            }
        });

        if (!response.ok) {
            throw new Error(`Failed to get passkey creation options: ${response.statusText}`);
        }

        const optionsJson = await response.json();
        
        // Parse creation options
        const options = PublicKeyCredential.parseCreationOptionsFromJSON(optionsJson);
        
        // Cancel any previous passkey operation
        if (abortController) {
            abortController.abort();
        }
        
        // Create new abort controller for this operation
        abortController = new AbortController();
        const signal = abortController.signal;
        
        // Prompt Windows Security for user to create passkey
        const credential = await navigator.credentials.create({ 
            publicKey: options,
            signal
        });

        if (!credential) {
            throw new Error('Failed to create credential');
        }
        
        // Serialize the credential to JSON format
        const serializedCredential = serializeCredential(credential);
        
        // Call backend to save the passkey
        const saveResponse = await fetch('/pages/authentication/AddOrUpdatePasskey', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': xsrf || ''
            },
            body: JSON.stringify({ credential: JSON.stringify(serializedCredential) })
        });

        if (!saveResponse.ok) {
            const errorText = await saveResponse.text();
            throw new Error(`Failed to save passkey: ${errorText}`);
        }

        const result = await saveResponse.json();
        
        return {
            success: true,
            credentialId: result.credentialId,
            credential: serializedCredential
        };
    } catch (error) {
        console.error('Error creating passkey:', error);
        throw error;
    }
}

/**
 * Gets Passkey request options for login
 * @param {string} username - Optional username
 */
export async function getPasskeyRequestOptions(username = null) {
    if (!browserSupportsPasskeys) {
        throw new Error('Browser does not support passkeys');
    }

    try {
        // Get XSRF token
        const xsrf = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        // Build URL
        const url = username 
            ? `/pages/authentication/PasskeyRequestOptions?username=${encodeURIComponent(username)}`
            : '/pages/authentication/PasskeyRequestOptions';

        // Call backend to get request options
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': xsrf || ''
            }
        });

        if (!response.ok) {
            throw new Error(`Failed to get passkey request options: ${response.statusText}`);
        }

        const optionsJson = await response.json();
        return optionsJson;
    } catch (error) {
        console.error('Error getting passkey request options:', error);
        throw error;
    }
}

/**
 * Authenticates using Passkey
 * @param {string} username - Optional username
 */
export async function getPasskeyCredential(username = null) {
    if (!browserSupportsPasskeys) {
        throw new Error('Browser does not support passkeys');
    }

    try {
        // Get request options
        const optionsJson = await getPasskeyRequestOptions(username);
        
        // Parse request options
        const options = PublicKeyCredential.parseRequestOptionsFromJSON(optionsJson);
        
        // Prompt Windows Security for user to select passkey for authentication
        const credential = await navigator.credentials.get({ 
            publicKey: options,
            mediation: 'optional'
        });

        if (!credential) {
            throw new Error('Failed to get credential');
        }
        
        // Serialize the credential for authentication
        const serializedCredential = serializeAuthenticationCredential(credential);
        
        return serializedCredential;
    } catch (error) {
        console.error('Error getting passkey credential:', error);
        throw error;
    }
}

/**
 * Checks if the browser supports passkeys
 */
export function checkPasskeySupport() {
    return browserSupportsPasskeys;
}

/**
 * Performs complete passkey login flow
 * Gets the passkey credential and sends it to the server for authentication
 * @param {string} username - Optional username
 * @returns {Promise<{success: boolean, redirectUrl?: string, error?: string}>}
 */
export async function loginWithPasskey(username = null) {
    if (!browserSupportsPasskeys) {
        throw new Error('Browser does not support passkeys');
    }

    try {
        // Get XSRF token for security
        const xsrf = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        // Get request options
        const optionsJson = await getPasskeyRequestOptions(username);
        
        // Parse request options
        const options = PublicKeyCredential.parseRequestOptionsFromJSON(optionsJson);
        
        // Prompt Windows Security for user to select passkey for authentication
        const credential = await navigator.credentials.get({ 
            publicKey: options,
            mediation: 'optional'
        });

        if (!credential) {
            throw new Error('Failed to get credential');
        }
        
        // Serialize the credential for authentication
        const serializedCredential = serializeAuthenticationCredential(credential);
        
        // Send credential to the server for authentication
        const loginResponse = await fetch('/pages/authentication/LoginWithPasskey', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': xsrf || ''
            },
            body: JSON.stringify({ credential: JSON.stringify(serializedCredential) })
        });

        if (!loginResponse.ok) {
            const errorText = await loginResponse.text();
            throw new Error(`Login failed: ${errorText}`);
        }

        // Check if there's a redirect in the response
        const redirectUrl = loginResponse.redirected ? loginResponse.url : '/';
        
        return {
            success: true,
            redirectUrl: redirectUrl
        };
    } catch (error) {
        console.error('Error during passkey login:', error);
        return {
            success: false,
            error: error.message
        };
    }
}