/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';

import { IconButton } from '@material-ui/core';
import { Menu } from '@material-ui/icons';

const styles = {
    menuButton: {
        marginLeft: -12,
        marginRight: 20,
    },
};

const DrawerToggleButton = (props) => {
    return (
        <div style={styles.container}>
            <IconButton 
                style={styles.menuButton}
                color="inherit" 
                onClick={props.onToggleClick}
            >
                <Menu/>
            </IconButton>
        </div>
    );
};

export default DrawerToggleButton;