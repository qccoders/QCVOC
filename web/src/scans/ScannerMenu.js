/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'proptypes';

import IconButton from '@material-ui/core/IconButton';
import { LockOpen, MoreVert }  from '@material-ui/icons';
import ConfirmDialog from '../shared/ConfirmDialog';
import { Menu, MenuItem, ListItemIcon, ListItemText } from '@material-ui/core';

const styles = {
    container: {
        display: 'inline',
    },
    caption: {
        marginRight: 5,
        display: 'inline'
    },
    icon: {
        fontSize: 29,
    },
    menu: {
        marginTop: 40,
    },
    iconMenu: {
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

    render() {
        let { menu } = this.state;

        return (
            <div style={styles.container}>
                <IconButton
                    color="inherit"
                    style={styles.iconMenu}
                    onClick={this.handleMenuClick}
                >
                    <MoreVert/>
                </IconButton>
                <Menu
                    open={menu.open}
                    anchorEl={menu.anchorEl}
                    onClose={this.handleMenuClose}
                    style={styles.menu}
                >
                    <MenuItem onClick={this.handleResetPasswordClick}>
                        <ListItemIcon>
                            <LockOpen/>
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
                    onConfirm={this.props.onReset}
                    onClose={this.handleConfirmDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want reset the Scanner?</p>
                </ConfirmDialog>
            </div>
        );
    }
}

ScannerMenu.propTypes = {
    onReset: PropTypes.func.isRequired,
}

export default ScannerMenu; 