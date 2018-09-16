/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import { PROD, DEV } from './environments';

export const sortByProp = (prop, predicate = 'asc') => {
    return (a, b) => {
        a = a[prop];
        b = b[prop];
        
        if (predicate === 'asc') {
            if (a > b) return 1;
            if (a < b) return -1;
            return 0;
        }
        else { 
            if (a > b) return -1;
            if (a < b) return 1;
            return 0;
        }
    }
}

export const getEnvironment = () => {
    if (window.location.hostname !== PROD.hostname) {
        return DEV;
    }

    return PROD;
}

export const getGuid = () => {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

export const validateEmail = (email) => {
    // eslint-disable-next-line
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

export const validatePhoneNumber = (phoneNumber) => {
    // eslint-disable-next-line
    var re = /\(\d{3}\) \d{3}-\d{4}/;
    return re.test(phoneNumber);
}

export const getUserRole = () => {
    const userRole = JSON.parse(localStorage.getItem('qcvoc-credentials')).role

    return userRole;
}