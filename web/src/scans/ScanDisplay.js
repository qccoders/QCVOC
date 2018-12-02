/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { 
    Card, 
    CardContent, 
    Typography, 
    CircularProgress, 
    Button,
} from '@material-ui/core';
import { SpeakerPhone, Today, Shop } from '@material-ui/icons';

import { getScanResult } from './scannerUtil';

const ScanResult = (props) => {
    let { scan } = props;

    if (scan === undefined || scan.status === undefined) return;

    let { veteran, plusOne } = scan.response;
    let { message, icon } = getScanResult(scan);

    icon = React.cloneElement(icon, { style: { fontSize: 72 }});
    let title = veteran ? veteran : scan.cardNumber;

    console.log(veteran);
    
    return (
        <div>
            <Typography component="h2" variant="display2" gutterBottom>{title}</Typography>
            {plusOne && <Typography component="h2" variant="display1" gutterBottom>+1</Typography>}
            {icon}
            <Typography style={{ marginTop: 20 }} variant="title" gutterBottom>{message}</Typography>
        </div>
    );
}

export default ScanResult; 