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

import { validateEmail, validatePhoneNumber } from '../util';
import api from '../api';

import CircularProgress from '@material-ui/core/CircularProgress';
import Snackbar from '@material-ui/core/Snackbar';
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
        isErrored: false,
    },
    deleteApi: {
        isExecuting: false,
        isErrored: false,
    },
    updateApi: {
        isExecuting: false,
        isErrored: false,
    },
    patron: {
        memberId: '',
        firstName: '',
        lastName: '',
        address: '',
        primaryPhone: '',
        secondaryPhone: '',
        email: '',
    },
    validation: {
        memberId: undefined,
        firstName: undefined,
        lastName: undefined,
        address: undefined,
        primaryPhone: undefined,
        secondaryPhone: undefined,
        email: undefined,
    },
    snackbar: {
        message: '',
        open: false,
    },
    confirmDialog: {
        open: false,
    },
}

class PatronDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState, 
                patron: nextProps.patron ? nextProps.patron : { 
                    ...initialState.patron, 
                },
                validation: initialState.validation,
            });
        }
    }

    handleSaveClick = () => {
        let patron = { ...this.state.patron }
        let fullName = patron.firstName + ' ' + patron.lastName;

        if (patron.email === '') delete patron.email;
        if (patron.secondaryPhone === '') delete patron.secondaryPhone;

        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => api.post('/v1/patrons', patron),
                        'addApi', 
                        'Veteran \'' + fullName + '\' successfully enrolled.'
                    )
                }
                else {
                    this.execute(
                        () => api.put('/v1/patrons/' + patron.id, patron), 
                        'updateApi', 
                        'Veteran \'' + fullName +  '\' successfully updated.'
                    );
                }
            }
        });
    }

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleDeleteClick = () => {
        this.setState({ confirmDialog: { open: true }});
    }

    handleDeleteConfirmClick = () => {
        return this.execute(
            () => api.delete('/v1/patrons/' + this.state.patron.id),
            'deleteApi', 
            'Veteran \'' + this.state.patron.firstName + ' ' + this.state.patron.lastName +  '\' successfully deleted.'
        );
    }

    handleChange = (field, event) => {
        this.setState({ 
            patron: {
                ...this.state.patron,
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

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    execute = (action, api, successMessage) => {
        return new Promise((resolve, reject) => {
            this.setState({ [api]: { isExecuting: true }}, () => {
                action()
                .then(response => {
                    this.setState({
                        [api]: { isExecuting: false, isErrored: false }
                    }, () => {
                        this.props.onClose(successMessage);
                        resolve(response);
                    })
                }, error => {
                    var body = error && error.response && error.response.data ? error.response.data : error;

                    if (typeof(body) !== 'string') {
                        var keys = Object.keys(body);
    
                        if (keys.length > 0) {
                            body = body[keys[0]];
                        }
                    }

                    this.setState({ 
                        [api]: { isExecuting: false, isErrored: true },
                        snackbar: {
                            message: body,
                            open: true,
                        },
                    }, () => reject(error));
                })
            })
        })
    }

    validate = () => {
        let { memberId, firstName, lastName, address, primaryPhone, secondaryPhone, email } = this.state.patron;
        let result = { ...initialState.validation };

        if (memberId === '') result.memberId = 'The Member ID field is required.';
        if (memberId !== '' && (isNaN(memberId) || memberId < 1000 || memberId > 9999)) result.memberId = 'The Member ID field must be a number between 1000 and 9999.';
        if (firstName === '') result.firstName = 'The First Name field is required.';
        if (lastName === '') result.lastName = 'The Last Name field is required.';
        if (address === '') result.address = 'The Address field is required.';
        if (address !== '' && address.length < 5) result.address = 'The Address field must be a minimum of 5 characters.';

        if (primaryPhone === '') {
            result.primaryPhone = 'The Primary Phone field is required.';
        }
        else if (!validatePhoneNumber(primaryPhone)) {
            result.primaryPhone = 'Enter a valid phone number in the format (555) 555-5555.';
        }

        if ((secondaryPhone !== '' && secondaryPhone !== undefined) && !validatePhoneNumber(secondaryPhone)) {
            result.secondaryPhone = 'Enter a valid phone number in the format (555) 555-5555.';
        }

        if ((email !== '' && email !== undefined) && !validateEmail(email)) {
            result.email = 'Enter a valid email address.';
        }

        return new Promise(resolve => {
            this.setState({ validation: result }, () => {
                result.isValid = JSON.stringify(result) === JSON.stringify(initialState.validation);
                resolve(result);
            });                
        })
    }

    render() {
        let { classes, intent, open } = this.props;
        let { memberId, firstName, lastName, address, primaryPhone, secondaryPhone, email } = this.state.patron;
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
                <DialogTitle>{(intent === 'add' ? 'Enroll' : 'Update')} Veteran</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        id="memberId"
                        label="Member ID"
                        value={memberId}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('memberId', event)}
                        helperText={validation.memberId}
                        error={validation.memberId !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField
                        id="firstName"
                        label="First Name"
                        value={firstName}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('firstName', event)}
                        helperText={validation.firstName}
                        error={validation.firstName !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField                        
                        id="lastName"
                        label="Last Name"
                        value={lastName}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('lastName', event)}
                        helperText={validation.lastName}
                        error={validation.lastName !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField                        
                        id="address"
                        label="Address"
                        value={address}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('address', event)}
                        helperText={validation.address}
                        error={validation.address !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField                        
                        id="email"
                        label="Email"
                        value={email}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('email', event)}
                        helperText={validation.email}
                        error={validation.email !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField                        
                        id="primaryPhone"
                        label="Primary Phone"
                        value={primaryPhone}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('primaryPhone', event)}
                        helperText={validation.primaryPhone}
                        error={validation.primaryPhone !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <TextField                        
                        id="secondaryPhone"
                        label="Secondary Phone"
                        value={secondaryPhone}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('secondaryPhone', event)}
                        helperText={validation.secondaryPhone}
                        error={validation.secondaryPhone !== undefined}
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
                    title={'Confirm Veteran Deletion'}
                    prompt={'Delete'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleDeleteConfirmClick}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to delete Patron '{firstName + ' ' + lastName}'?</p>
                </ConfirmDialog>
                <Snackbar
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'center'}}
                    open={this.state.snackbar.open}
                    onClose={this.handleSnackbarClose}
                    autoHideDuration={3000}
                    message={<span id="message-id">{this.state.snackbar.message}</span>}
                />
            </Dialog>
        );
    }
}

PatronDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'update' ]).isRequired,
    open: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    patron: PropTypes.object,
};

export default withStyles(styles)(PatronDialog); 