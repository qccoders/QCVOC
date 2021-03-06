/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';

import { Typography } from '@material-ui/core';

import ContentWrapper from '../../shared/ContentWrapper';

const Error = (props) => {
    return (
        <ContentWrapper api={{ isExecuting: false, isErrored: false }}>
            <div style={{ textAlign: 'center' }}>
                {props.children}
                <Typography 
                    component="h2" 
                    variant="subtitle1" 
                    gutterBottom
                    style={{ marginTop: 50 }}
                >
                    Click <a href="/">here</a> to return to the home page.
                </Typography>
            </div>
        </ContentWrapper>
    );
};

export default Error; 