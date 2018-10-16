/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';

import { AppBar as MaterialAppBar } from '@material-ui/core';
import { Toolbar, Typography } from '@material-ui/core';

const styles = {
    appBar: {
        marginBottom: 10,
    },
    title: {
        marginLeft: -12,
    },
};

const AppBar = (props) => {
    return (
        <MaterialAppBar 
            position="static" 
            color={props.color ? props.color : "primary"}
            style={styles.appBar}
        >
            <Toolbar>
                {props.drawerToggleButton}
                <Typography variant="title" color="inherit" style={styles.title}>
                    {props.title}
                </Typography>
                {props.children}
            </Toolbar>
        </MaterialAppBar>
    );
};

AppBar.propTypes = {
    title: PropTypes.string,
    drawerToggleButton: PropTypes.object,
    children: PropTypes.object,
    color: PropTypes.string,
};

export default AppBar; 