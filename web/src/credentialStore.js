/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

const CREDENTIAL_KEY = 'qcvoc-credentials';

export const getCredentials = () => {
    var credentials = JSON.parse(sessionStorage.getItem(CREDENTIAL_KEY));

    if (credentials) {
        return credentials;
    } else {
        return JSON.parse(localStorage.getItem(CREDENTIAL_KEY));
    }
};

export const saveSessionCredentials = (credentials) => {
    sessionStorage.setItem(CREDENTIAL_KEY, JSON.stringify(credentials));
};

export const saveLocalCredentials = (credentials) => {
    localStorage.setItem(CREDENTIAL_KEY, JSON.stringify(credentials));
};

export const updateCredentials = (credentials) => {
    if (JSON.parse(sessionStorage.getItem(CREDENTIAL_KEY))) {
        sessionStorage.setItem(CREDENTIAL_KEY, JSON.stringify(credentials));
    } else {
        localStorage.setItem(CREDENTIAL_KEY, JSON.stringify(credentials));
    }
};

export const deleteCredentials = () => {
    sessionStorage.removeItem(CREDENTIAL_KEY);
    localStorage.removeItem(CREDENTIAL_KEY);
};