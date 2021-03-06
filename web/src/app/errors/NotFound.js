/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';

import { Typography } from '@material-ui/core';
import { Clear } from '@material-ui/icons';

import Error from './Error';

const NotFound = (props) => {
    return (
        <Error>
            <Clear style={{ fontSize: 72 }}/>
            <Typography variant="h4" gutterBottom>
                Not Found
            </Typography>
            <Typography variant="subtitle1" gutterBottom>
                The specified resource does not exist.
            </Typography>
        </Error>
    );
};

export default NotFound; 