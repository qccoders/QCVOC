/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { 
    Avatar,
    Button,
    Dialog,
    DialogTitle,
    DialogActions,
    DialogContent,
    IconButton,
    List,
    ListItem,
    ListItemText,
    ListItemSecondaryAction,
} from '@material-ui/core';
import { Delete } from '@material-ui/icons';

import { getScanResult } from './scannerUtil';
import { withContext } from '../shared/ContextProvider';
import ConfirmDialog from '../shared/ConfirmDialog';

const styles = {
    dialog: {
        width: 320,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
};

const initialState = {
    api: {
        isExecuting: false,
        isErrored: false,
    },
    confirmDialog: {
        open: false,
        scan: undefined,
    },
};

class ScannerHistoryDialog extends Component {
    state = initialState;

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleDeleteClick = (scan) => {
        this.setState({ 
            ...this.state,
            confirmDialog: {
                open: true,
                scan: scan,
            },
        });
    }

    handleDeleteConfirmation = () => {
        return new Promise((resolve, reject) => {
            this.setState({ api: { isExecuting: true}}, () => {
                this.props.context.api.delete('/v1/scans/')
                .then(response => {
                    this.setState({ api: { isExecuting: false, isErrored: false }}, resolve(response));
                }, error => {
                    this.setState({ api: { isExecuting: false, isErrored: true }}, () => reject(error));
                });
            });
        });
    }

    handleConfirmDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }
   
    render() {
        let { classes, open, history } = this.props;

        let selectedScan = this.state.confirmDialog.scan;
        let selectedVeteran = selectedScan && selectedScan.response && selectedScan.response.veteran ? selectedScan.response.veteran.fullName : '';

        return (
            <Dialog 
                open={open}
                onClose={this.handleCancelClick}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}                
            >
                <DialogTitle>Scanner History</DialogTitle>
                <DialogContent>
                    <List>
                        {history.map((scan, index) => 
                            <ListItem key={index}>
                                <Avatar style={{ backgroundColor: getScanResult(scan).color }}>{getScanResult(scan).icon}</Avatar>
                                <ListItemText 
                                    primary={(scan.response.veteran && scan.response.veteran.fullName ? scan.response.veteran.fullName : scan.cardNumber) + ' ' + (scan.response.plusOne ? '+1' : '')}
                                    secondary={getScanResult(scan).message}
                                />
                                <ListItemSecondaryAction>
                                    <IconButton onClick={() => this.handleDeleteClick(scan)}>
                                        <Delete/>
                                    </IconButton>
                                </ListItemSecondaryAction>
                            </ListItem>
                        )}
                    </List>
                </DialogContent>
                <DialogActions>
                    <Button 
                        onClick={this.handleCancelClick}
                        color="primary"
                    >
                        Close
                    </Button>
                </DialogActions>
                <ConfirmDialog
                    title={'Confirm Scan Deletion'}
                    prompt={'Delete'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleDeleteConfirmation}
                    onClose={this.handleConfirmDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to delete the Scan for '{selectedVeteran}'?</p>
                </ConfirmDialog>
            </Dialog>
        );
    }
}

ScannerHistoryDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    open: PropTypes.bool.isRequired,
    history: PropTypes.arrayOf(PropTypes.object),
};

export default withStyles(styles)(withContext(ScannerHistoryDialog)); 