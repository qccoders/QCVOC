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

    handleDeleteConfirmation = (scan) => {
        let eventId = scan.response.eventId;
        let veteranId = scan.response.veteranId;
        let serviceId = scan.response.serviceId;

        let url = '/v1/scans/' + eventId + '/';

        if (serviceId !== undefined) {
            url = url + serviceId + '/';
        }

        url = url + veteranId;

        return new Promise((resolve, reject) => {
            this.setState({ api: { isExecuting: true}}, () => {
                this.props.context.api.delete(url)
                .then(response => {
                    this.setState({ api: { isExecuting: false, isErrored: false }}, () => {
                        this.props.onDelete(scan);
                        this.props.context.showMessage('Scan(s) for ' + scan.response.veteran.fullName + ' deleted.');
                        resolve(response);
                    });
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
                                {getScanResult(scan).accepted && <ListItemSecondaryAction>
                                    <IconButton onClick={() => this.handleDeleteClick(scan)}>
                                        <Delete/>
                                    </IconButton>
                                </ListItemSecondaryAction>}
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
                    onConfirm={() => this.handleDeleteConfirmation(selectedScan)}
                    onClose={this.handleConfirmDialogClose}
                >
                    <p>Are you sure you want to delete {this.props.service} Scan(s) for '{selectedVeteran}'?</p>
                </ConfirmDialog>
            </Dialog>
        );
    }
}

ScannerHistoryDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    open: PropTypes.bool.isRequired,
    service: PropTypes.string.isRequired,
    history: PropTypes.arrayOf(PropTypes.object),
    onDelete: PropTypes.func.isRequired,
};

export default withStyles(styles)(withContext(ScannerHistoryDialog)); 