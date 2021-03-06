/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import axios from 'axios';
import { getCredentials, updateCredentials, deleteCredentials } from './credentials';
import { getEnvironment } from './util';

axios.defaults.baseURL = getEnvironment().apiRoot;

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

    if (error.response.status === 401) {
        if (creds && creds.refreshToken) {
            let data = JSON.stringify(creds.refreshToken);
            let headers = { headers: { 'Content-Type': 'application/json' }};

            // use 'axios' here instead of the 'api' instance we created to bypass our interceptors
            // and avoid an endless loop should this call result in a 401.
            return axios.post('/v1/security/refresh', data, headers)
                .then(response => {
                    updateCredentials(response.data);
                    request.headers.Authorization = response.data.tokenType + ' ' + response.data.accessToken;
                    return api(request);
                }, error => {
                    deleteCredentials();
                    window.location.reload(true);
                    return Promise.reject(error);
                }
            );
        }

        window.location.reload(true);
        return Promise.reject(error);
    } 
    else {
        logError(error);
        return Promise.reject(error);
    }
});

const logError = (error) => {
    if (error.response) {
        console.log(error.response.data);
        console.log(error.response.status);
        console.log(error.response.headers);
    } else if (error.request) {
        console.log(error.request);
    } else {
        console.log('Error: ', error.message);
    }
    console.log(error.config);
};

export default api;