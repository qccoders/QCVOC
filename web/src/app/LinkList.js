/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';

const LinkList = (props) => {
    return props.children
        .filter(child => child)
        .map((child, index) => React.cloneElement(child, { key: index, onClick: props.onLinkClick }));
};

LinkList.propTypes = {
    onLinkClick: PropTypes.func.isRequired,
};

export default LinkList; 