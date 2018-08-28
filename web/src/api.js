/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import axios from 'axios';
import { getCredentials, saveCredentials, deleteCredentials } from './credentialStore';
import { API_ROOT } from './constants';

axios.defaults.baseURL = API_ROOT;

const api = axios.create();

api.interceptors.request.use(config => {
    let creds = getCredentials();

    config.headers['Content-Type'] = 'application/json';

    if (creds && creds.accessToken) {
        config.headers.Authorization = creds.tokenType + ' ' + creds.accessToken;
    }

    return config;
});

api.interceptors.response.use(config => {
    return config;
}, error => {
    let request = error.config;
    let creds = getCredentials();

    if (error.response.status === 401 && creds && creds.refreshToken) {
        let data = JSON.stringify(creds.refreshToken);
        let headers = { headers: { 'Content-Type': 'application/json' }};

        // use 'axios' here instead of the 'api' instance we created to bypass our interceptors
        // and avoid an endless loop should either of these two calls result in a 401.
        return axios.post('/v1/security/refresh', data, headers)
            .then(response => {
                saveCredentials(response.data);
                request.headers.Authorization = response.data.tokenType + ' ' + response.data.accessToken;
                return axios(request);
            }, error => {
                deleteCredentials();
                window.location.reload(true);
                return Promise.reject(error);
            }
        );
    } 
    else {
        return Promise.reject(error);
    }
})

export default api;