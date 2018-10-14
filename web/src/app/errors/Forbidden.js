/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';

import Error from './Error';
import { Typography } from '@material-ui/core';

const Forbidden = (props) => {
    return (
        <Error>
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