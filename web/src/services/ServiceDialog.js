/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import {
    Dialog,
    DialogTitle,
    DialogActions,
    Button,
    DialogContent,
    TextField,
} from '@material-ui/core';

import { withContext } from '../shared/ServiceContext';

import CircularProgress from '@material-ui/core/CircularProgress';
import ConfirmDialog from '../shared/ConfirmDialog';

const styles = {
    dialog: {
        width: 320,
        marginRight: 'auto',
        marginLeft: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
    deleteButton: {
        marginRight: 'auto',
    },
    spinner: {
        position: 'fixed',
    },
};

const initialState = {
    addApi: {
        isExecuting: false,
        isErrored: false
    },
    deleteApi: {
        isExecuting: false,
        isErrored: false,
    },
    updateApi: {
        isExecuting: false,
        isErrored: false,
    },
    service: {
        name: '',
        description: '',
    },
    validation: {
        name: undefined,
        description: undefined,
    },
    confirmDialog: {
        open: false,
    },
}

class ServiceDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({
                ...initialState,
                service: nextProps.service ? nextProps.service : {
                    ...initialState.service,
                },
                validation: initialState.validation,
            });
        }
    }

    handleSaveClick = () => {
        let service = { ...this.state.service }

        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => this.props.context.apiPost('/v1/services', service),
                        'addApi',
                        'Service \'' + service.name + '\' successfully created.'
                    )
                }
                else {
                    this.execute(
                        () => this.props.context.apiPut('/v1/services/' + service.id, service),
                        'updateApi',
                        'Service \'' + service.name + '\' successfully updated.'
                    );
                }
            }
        });
    }

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleDeleteClick = () => {
        this.setState({ confirmDialog: {open: true }});
    }

    handleDeleteConfirmClick = () => {
        return this.execute(
            () => this.props.context.apiDelete('/v1/services/' + this.state.service.id),
            'deleteApi',
            'Service \'' + this.state.service.name + '\' successfully deleted.'
        );
    }

    handleChange = (field, event) => {
        this.setState({
            service: {
                ...this.state.service,
                [field]: event.target.value,
            },
            validation: {
                ...this.state.validation,
                [field]: undefined,
            },
        });
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    execute = (action, api, successMessage) => {
        return new Promise((resolve, reject) => {
            this.setState({ [api]: {isExecuting: true }}, () => {
                action()
                .then(response => {
                    this.setState({
                        [api]: { isExecuting: false, isErrored: false }
                    }, () => {
                        this.props.onClose(successMessage);
                        resolve(response);
                    })
                }, error => {
                    this.setState({ [api]: { isExecuting: false, isErrored: true } }, () => reject(error));
                })
            })
        })
    }

    validate = () => {
        let { name, description } = this.state.service;
        let result = { ...initialState.validation };

        if (name === '') result.name = "The Name field is required.";
        if (description === '') result.description = "The Description field is required.";

        return new Promise(resolve => {
            this.setState({ validation: result }, () => {
                result.isValid = JSON.stringify(result) === JSON.stringify(initialState.validation);
                resolve(result);
            });
        })
    }

    render() {
        let { classes, intent, open } = this.props;
        let { name, description } = this.state.service;
        let validation = this.state.validation;

        let adding = this.state.addApi.isExecuting;
        let updating = this.state.updateApi.isExecuting;
        let deleting = this.state.deleteApi.isExecuting;
        
        let executing = adding || updating || deleting;
        
        return (
            <Dialog 
                open={open}
                onClose={this.handleCancelClick}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}
            >
                <DialogTitle>{(intent === 'add' ? 'Create' : 'Update')} Service</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        id="name"
                        label="Name"
                        value={name}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('name', event)}
                        helperText={validation.name}
                        error={validation.name !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField                        
                        id="description"
                        label="Description"
                        value={description}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('description', event)}
                        helperText={validation.description}
                        error={validation.description !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                </DialogContent>
                <DialogActions>
                    {intent === 'update' && 
                        <Button 
                            onClick={this.handleDeleteClick} 
                            color="primary" 
                            className={classes.deleteButton}
                            disabled={executing}
                        >
                            {deleting && <CircularProgress size={20} style={styles.spinner}/>}
                            Delete
                        </Button>
                    }
                    <Button 
                        onClick={this.handleCancelClick}
                        color="primary"
                        disabled={executing}
                    >
                        Cancel
                    </Button>
                    <Button 
                        onClick={this.handleSaveClick} 
                        color="primary"
                        disabled={executing}
                    >
                        {(adding || updating) && <CircularProgress size={20} style={styles.spinner}/>}
                        {intent === 'add' ? 'Add' : 'Update'}
                    </Button>
                </DialogActions>
                <ConfirmDialog
                    title={'Confirm Service Deletion'}
                    prompt={'Delete'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleDeleteConfirmClick}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to delete Service '{name}'?</p>
                </ConfirmDialog>
            </Dialog>
        );
    }
}

ServiceDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'update' ]).isRequired,
    open: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    service: PropTypes.object,
};

export default withStyles(styles)(withContext(ServiceDialog));