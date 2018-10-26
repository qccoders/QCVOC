/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';

import { Typography } from '@material-ui/core';
import { Block } from '@material-ui/icons';

import Error from './Error';

const Forbidden = (props) => {
    return (
        <Error>
            <Block style={{ fontSize: 72 }}/>
            <Typography component="h2" variant="display1" gutterBottom>
                Forbidden
            </Typography>
            <Typography component="h2" variant="subheading" gutterBottom>
                You don't have permission to view the requested resource.
            </Typography>
        </Error>
    );
};

export default Forbidden; 