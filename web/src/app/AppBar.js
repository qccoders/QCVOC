/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';

import { AppBar as MaterialAppBar } from '@material-ui/core';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';

const AppBar = (props) => {
    return (
        <MaterialAppBar position="static" color={props.color ? props.color : "primary"}>
            <Toolbar>
                {props.drawerToggleButton}
                <Typography variant="title" color="inherit">
                    {props.title}
                </Typography>
                {props.children}
            </Toolbar>
        </MaterialAppBar>
    );
}

AppBar.propTypes = {
    title: PropTypes.string,
    drawerToggleButton: PropTypes.object,
    children: PropTypes.object,
    color: PropTypes.string,
};

export default AppBar; 