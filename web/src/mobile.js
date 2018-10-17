/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

export const isMobileAttached = () => {
    return isAndroidAttached();
};

export const isAndroidAttached = () => {
    return window["Android"] !== undefined;
};

export const initiateMobileScan = (callback) => {
    if (isAndroidAttached()) {
        initiateAndroidScan(callback);
    }
};

export const initiateAndroidScan = (callback) => {
    window["Android"]["scanBarcode"](callback);
};