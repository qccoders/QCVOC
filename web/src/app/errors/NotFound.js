/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import ContentWrapper from '../../shared/ContentWrapper';

const NotFound = (props) => {
    return (
        <ContentWrapper api={{ isExecuting: false, isErrored: false }}>
            <p>Not Found</p>
        </ContentWrapper>
    );
};

export default NotFound; 