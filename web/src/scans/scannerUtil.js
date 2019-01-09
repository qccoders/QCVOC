import React from 'react';

import { red, green, yellow } from '@material-ui/core/colors';
import { Done, Clear, Pause } from '@material-ui/icons';

export const getScanResult = (scan) => {
    switch(scan.status) {
        case undefined:
            return undefined;
        case 201:
            return { accepted: true, message: 'Scan Accepted', color: green['A700'], icon: <Done/> };
        case 403:
            return { accepted: false, message: 'The Veteran has not checked in for this Event.', color: red['A700'], icon: <Clear/> };
        case 409:
            return { accepted: false, message: 'Duplicate Scan', color: yellow['A700'], icon: <Pause/> };
        default:
            return { accepted: false, message: scan.response, color: red['A700'], icon: <Clear/> };
    }
};