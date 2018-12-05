/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

export const isMobileAttached = () => {
    return isAndroidAttached();
};

export const isAndroidAttached = () => {
    return navigator.userAgent.includes('Android');
};

export const initiateMobileScan = (callback) => {
    if (isAndroidAttached()) {
        initiateAndroidScan(callback);
    }
};

export const initiateMobilePhotoAcquisition = (callback) => {
    if (isAndroidAttached()) {
        initiateAndroidPhotoAcquisition(callback);
    }
};

export const initiateAndroidScan = (callback) => {
    redirectToQuery('?scan&callback=' + callback);
};

export const initiateAndroidPhotoAcquisition = (callback) => {
    redirectToQuery('?acquirePhoto&callback=' + callback);
};

const redirectToQuery = (query) => {
    let arr = window.location.toString().split('?');
    window.location.assign(arr[0] + query);
};