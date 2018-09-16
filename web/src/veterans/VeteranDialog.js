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
    FormControl,
    InputLabel,
    Select,
    MenuItem,
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
    verificationSelect: {
        marginTop: 15,
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
    veteran: {
        cardNumber: '',
        firstName: '',
        lastName: '',
        address: '',
        primaryPhone: '',
        email: '',
        verificationMethod: '',
    },
    validation: {
        cardNumber: undefined,
        firstName: undefined,
        lastName: undefined,
        address: undefined,
        primaryPhone: undefined,
        email: undefined,
        verificationMethod: undefined,
    },
    snackbar: {
        message: '',
        open: false,
    },
    confirmDialog: {
        open: false,
    },
}

class VeteranDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState, 
                veteran: nextProps.veteran ? nextProps.veteran : { 
                    ...initialState.veteran, 
                },
                validation: initialState.validation,
            });
        }
    }

    handleSaveClick = () => {
        let veteran = { ...this.state.veteran }
        let fullName = veteran.firstName + ' ' + veteran.lastName;

        if (veteran.email === '') delete veteran.email;
        if (veteran.cardNumber === '') delete veteran.cardNumber;

        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => api.post('/v1/veterans', veteran),
                        'addApi', 
                        'Veteran \'' + fullName + '\' successfully enrolled.'
                    )
                }
                else {
                    this.execute(
                        () => api.put('/v1/veterans/' + veteran.id, veteran), 
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
            () => api.delete('/v1/veterans/' + this.state.veteran.id),
            'deleteApi', 
            'Veteran \'' + this.state.veteran.firstName + ' ' + this.state.veteran.lastName +  '\' successfully deleted.'
        );
    }

    handleChange = (field, event) => {
        this.setState({ 
            veteran: {
                ...this.state.veteran,
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
        let { cardNumber, firstName, lastName, address, primaryPhone, email } = this.state.veteran;
        let result = { ...initialState.validation };

        if (cardNumber !== '' && (isNaN(cardNumber) || cardNumber < 1000 || cardNumber > 9999)) result.cardNumber = 'The Card Number field must be a number between 1000 and 9999.';
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
        let { cardNumber, firstName, lastName, address, primaryPhone, email, verificationMethod } = this.state.veteran;
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
                        id="cardNumber"
                        label="Card Number"
                        value={cardNumber}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('cardNumber', event)}
                        helperText={validation.cardNumber}
                        error={validation.cardNumber !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    />
                    <FormControl 
                        className={classes.verificationSelect}
                        fullWidth
                        disabled={executing}
                    >
                        <InputLabel>Verification Method</InputLabel>
                        <Select
                            value={verificationMethod}
                            onChange={(event) => this.handleChange('verificationMethod', event)}
                            fullWidth
                        >
                            <MenuItem value={1}>State ID and DD214</MenuItem>
                            <MenuItem value={2}>Iowa Veteran's Driver's License</MenuItem>
                            <MenuItem value={3}>Active Military ID</MenuItem>
                        </Select>
                    </FormControl>
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
                    <p>Are you sure you want to delete Veteran '{firstName + ' ' + lastName}'?</p>
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

VeteranDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'update' ]).isRequired,
    open: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    veteran: PropTypes.object,
};

export default withStyles(styles)(VeteranDialog); 