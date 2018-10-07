import React from 'react';
import { red, green, yellow } from '@material-ui/core/colors';
import { Done, Clear, Pause } from '@material-ui/icons';

export const getScanResult = (scan) => {
    switch(scan.status) {
        case undefined:
            return undefined;
        case 201:
            return { message: 'Scan Accepted', color: green['A700'], icon: <Done/> };
        case 200:
            return { message: 'Duplicate Scan', color: yellow['A700'], icon: <Pause/> };
        default:
            return { message: 'Invalid Card', color: red['A700'], icon: <Clear/> };
    }
}