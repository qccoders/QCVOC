/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'proptypes';
import { withStyles } from '@material-ui/core/styles';

import IconButton from '@material-ui/core/IconButton';
import { MoreVert, Replay }  from '@material-ui/icons';
import ConfirmDialog from '../shared/ConfirmDialog';
import { Menu, MenuItem, ListItemIcon, ListItemText } from '@material-ui/core';

const styles = {
    container: {
        display: 'inline',
    },
    iconButton: {
        float: 'right',
        marginTop: -8,
    },
}

const initialState = {
    menu: {
        anchorEl: undefined,
        open: false,
    },
    confirmDialog: {
        open: false,
    },
};

class ScannerMenu extends Component {
    state = initialState;

    handleMenuClick = (event) => {
        this.setState({ menu: { anchorEl: event.currentTarget, open: true }});
    }

    handleMenuClose = () => {
        this.setState({ menu: { open: false }});
    }

    handleConfirmDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    handleResetScannerClick = () => {
        this.setState({ menu: { open: false }, confirmDialog: { open: true }});
    }

    resetScanner = () => {
        return new Promise((resolve) => {
            this.props.resetScanner(resolve);
        });
    }

    render() {
        let { classes } = this.props;
        let { menu } = this.state;

        return (
            <div className={classes.container}>
                <IconButton
                    color="inherit"
                    className={classes.iconButton}
                    onClick={this.handleMenuClick}
                >
                    <MoreVert/>
                </IconButton>
                <Menu
                    open={menu.open}
                    anchorEl={menu.anchorEl}
                    onClose={this.handleMenuClose}
                >
                    <MenuItem onClick={this.handleResetScannerClick}>
                        <ListItemIcon>
                            <Replay/>
                        </ListItemIcon>
                        <ListItemText>
                            Reset Scanner
                        </ListItemText>
                    </MenuItem>
                </Menu>
                <ConfirmDialog
                    title={'Confirm Reset'}
                    prompt={'Reset'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.resetScanner}
                    onClose={this.handleConfirmDialogClose}
                >
                    <p>Are you sure you want reset the Scanner?</p>
                </ConfirmDialog>
            </div>
        );
    }
}

ScannerMenu.propTypes = {
    resetScanner: PropTypes.func.isRequired,
}

export default withStyles(styles)(ScannerMenu); 