/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'proptypes';

import { withStyles } from '@material-ui/core/styles';
import { 
    Menu, 
    MenuItem, 
    ListItemIcon, 
    ListItemText, 
    Divider, 
    IconButton,
} from '@material-ui/core';
import { MoreVert, Replay, History, ArrowBack }  from '@material-ui/icons';

import ConfirmDialog from '../shared/ConfirmDialog';

const styles = {
    container: {
        display: 'inline',
    },
    iconButton: {
        float: 'right',
        marginTop: -8,
    },
};

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
        this.close();
    }

    handleConfirmDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    handleResetScannerClick = () => {
        this.close().then(() => this.setState({ confirmDialog: { open: true }}));
    }

    handleHistoryClick = () => {
        this.close().then(() => this.props.viewHistory());
    }

    handleClearLastScanClick = () => {
        this.close().then(() => this.props.clearLastScan());
    }

    resetScanner = () => {
        return new Promise((resolve) => {
            this.props.resetScanner(resolve);
        });
    }

    close = () => {
        return new Promise(resolve => {
            this.setState({ menu: { open: false }}, () => resolve());
        });
    }

    render() {
        let { classes, visible, configured, lastScan } = this.props;
        let { menu } = this.state;

        return (
            <div className={classes.container}>
                {visible && <IconButton
                    color="inherit"
                    className={classes.iconButton}
                    onClick={this.handleMenuClick}
                >
                    <MoreVert/>
                </IconButton>}
                <Menu
                    open={menu.open}
                    anchorEl={menu.anchorEl}
                    onClose={this.handleMenuClose}
                >
                    {configured && 
                        <div style={{ outline: 'none' }}>
                            {lastScan && lastScan.status && <MenuItem onClick={this.handleClearLastScanClick}>
                                <ListItemIcon>
                                    <Replay/>
                                </ListItemIcon>
                                <ListItemText>
                                    Clear Last Scan
                                </ListItemText>
                            </MenuItem>}
                            <MenuItem onClick={this.handleHistoryClick}>
                                <ListItemIcon>
                                    <History/>
                                </ListItemIcon>
                                <ListItemText>
                                    View Scan History
                                </ListItemText>
                            </MenuItem>
                        </div>
                    }
                    <Divider/>
                    <MenuItem onClick={this.handleResetScannerClick}>
                        <ListItemIcon>
                            <ArrowBack/>
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
    visible: PropTypes.bool,
    configured: PropTypes.bool,
    resetScanner: PropTypes.func.isRequired,
    lastScan: PropTypes.object,
    clearLastScan: PropTypes.func.isRequired,
    viewHistory: PropTypes.func.isRequired,
};

ScannerMenu.defaultProps = {
    visible: true,
    configured: false,
};

export default withStyles(styles)(ScannerMenu); 