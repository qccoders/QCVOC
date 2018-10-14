/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import ContentWrapper from '../../shared/ContentWrapper';

import { Typography } from '@material-ui/core';

const Error = (props) => {
    return (
        <ContentWrapper api={{ isExecuting: false, isErrored: false }}>
            <div style={{ textAlign: 'center' }}>
                {props.children}
                <Typography 
                    component="h2" 
                    variant="subheading" 
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