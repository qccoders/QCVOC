/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { 
    Typography, 
    Avatar,
} from '@material-ui/core';

import { getScanResult } from './scannerUtil';

const styles = {
    photo: {
        width: 240,
        height: 240,
        margin: 'auto',
    },
    title: {
        marginTop: 10,
    },
    message: {
        marginTop: 10,
    },
};

const ScanResult = (props) => {
    let { classes, scan } = props;

    if (scan === undefined || scan.status === undefined) return;

    let { veteran, plusOne } = scan.response;
    let { message, icon } = getScanResult(scan);

    icon = React.cloneElement(icon, { style: { fontSize: 72 }});
    let title = veteran ? veteran.firstName + ' ' + veteran.lastName : scan.cardNumber;

    let photoPresent = veteran !== undefined && veteran.photoBase64 !== undefined && veteran.photoBase64 !== '';
    
    return (
        <div>
            {photoPresent ? <Avatar className={classes.photo} src={veteran.photoBase64}/> : icon}
            <Typography className={classes.title} variant="h5" gutterBottom>{title}</Typography>
            {plusOne && <Typography variant="h4" gutterBottom>+1</Typography>}
            <Typography className={classes.message} variant="h6" gutterBottom>{message}</Typography>
        </div>
    );
};

export default withStyles(styles)(ScanResult); 