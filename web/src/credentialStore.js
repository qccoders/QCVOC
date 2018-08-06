const CREDENTIAL_KEY = 'qcvoc-credentials';

export const getCredentials = () => {
    return JSON.parse(localStorage.getItem(CREDENTIAL_KEY));
}

export const saveCredentials = (credentials) => {
    localStorage.setItem(CREDENTIAL_KEY, JSON.stringify(credentials));
}

export const deleteCredentials = () => {
    localStorage.removeItem(CREDENTIAL_KEY);
}