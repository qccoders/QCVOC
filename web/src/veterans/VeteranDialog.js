/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
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
    CircularProgress,
    Grid,
    Avatar,
} from '@material-ui/core';

import { SpeakerPhone, PhotoCamera } from '@material-ui/icons';

import { validateEmail, validatePhoneNumber, userCanView } from '../util';
import { withContext } from '../shared/ContextProvider';
import ConfirmDialog from '../shared/ConfirmDialog';
import { isMobileAttached, initiateMobileScan } from '../mobile';

const styles = {
    avatar: {
        backgroundColor:'#f50057',
        top: 25,
        left: 10,
    },
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
    scanButton: {
        color: '#fff',
    },
    photo: {
        width: 240,
        height: 240,
        margin: 'auto',
        backgroundColor: '#E0E0E0',
    },
};

const initialState = {
    getApi: {
        isExecuting: false,
        isErrored: false,
    },
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
        verificationMethod: 'Unverified',
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
    confirmDialog: undefined,
};

class VeteranDialog extends Component {
    state = initialState;

    componentDidMount = () => {
        window.inputBarcodeVeteranDialog = this.handleBarcodeScanned;
    }

    handleScanClick = () => {
        if (isMobileAttached()) {
            initiateMobileScan("window.inputBarcodeVeteranDialog");
        }
    }

    handleBarcodeScanned = (barcode) => {
        if (barcode === undefined) return;

        this.setState({ 
            veteran: {
                ...this.state.veteran,
                cardNumber: barcode,
            },
            validation: {
                ...this.state.validation,
                cardNumber: undefined,
            },
        });
    }

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState, 
                veteran: nextProps.veteran ? nextProps.veteran : { 
                    ...initialState.veteran,
                },
                validation: initialState.validation,
            }, () => {
                if (this.state.veteran.id !== initialState.veteran.id) {
                    this.setState({ getApi: { isExecuting: true, isErrored: false }}, () => {
                        this.props.context.api.get('/v1/veterans/' + nextProps.veteran.id)
                        .then(response => this.setState({ 
                            veteran: response.data,
                            getApi: { isExecuting: false, isErrored: false }, 
                        }), error => this.setState({ getApi: { isExecuting: false, isErrored: true }}));
                    });
                }
            });
        }
    }

    handleSaveClick = () => {
        let veteran = { ...this.state.veteran };
        let fullName = veteran.firstName + ' ' + veteran.lastName;

        if (veteran.email === '') delete veteran.email;
        if (veteran.cardNumber === '') delete veteran.cardNumber;

        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => this.props.context.api.post('/v1/veterans', veteran),
                        'addApi', 
                        'Veteran \'' + fullName + '\' successfully enrolled.'
                    );
                }
                else {
                    if (this.props.veteran.cardNumber 
                        && this.props.veteran.cardNumber !== veteran.cardNumber) {
                        this.setState({ confirmDialog: 'changeCardNumber' });
                    } else {
                        this.execute(
                            () => this.props.context.api.put('/v1/veterans/' + veteran.id, veteran), 
                            'updateApi', 
                            'Veteran \'' + fullName +  '\' successfully updated.'
                        );
                    }
                }
            }
        });
    }

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleDeleteClick = () => {
        this.setState({ confirmDialog: 'delete'});
    }

    handleDeleteConfirmClick = () => {
        return this.execute(
            () => this.props.context.api.delete('/v1/veterans/' + this.state.veteran.id),
            'deleteApi', 
            'Veteran \'' + this.state.veteran.firstName + ' ' + this.state.veteran.lastName +  '\' successfully deleted.'
        );
    }

    handleUpdateConfirmClick = () => {
        let veteran = { ...this.state.veteran };
        let fullName = veteran.firstName + ' ' + veteran.lastName;

        return this.execute(
            () => this.props.context.api.put('/v1/veterans/' + veteran.id, veteran), 
            'updateApi', 
            'Veteran \'' + fullName +  '\' successfully updated.'
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
        this.setState({ confirmDialog: undefined});
    }

    execute = (action, api, successMessage) => {
        return new Promise((resolve, reject) => {
            this.setState({ [api]: { isExecuting: true }}, () => {
                action()
                .then(response => {
                    this.setState({
                        [api]: { isExecuting: false, isErrored: false },
                    }, () => {
                        this.props.onClose(successMessage);
                        resolve(response);
                    });
                }, error => {
                    this.setState({ 
                        [api]: { isExecuting: false, isErrored: true },
                    }, () => reject(error));
                });
            });
        });
    }

    validate = () => {
        let { cardNumber, firstName, lastName, address, primaryPhone, email } = this.state.veteran;
        let result = { ...initialState.validation };

        if (cardNumber !== '' && (isNaN(cardNumber) || cardNumber < 0 || cardNumber > 9999)) result.cardNumber = 'The Card Number field must be a number between 0 and 9999.';
        if (firstName === '') result.firstName = 'The First Name field is required.';
        if (lastName === '') result.lastName = 'The Last Name field is required.';
        if (address === '') result.address = 'The Address field is required.';
        if (address !== '' && address.length < 5) result.address = 'The Address field must be a minimum of 5 characters.';

        if (primaryPhone === '') {
            result.primaryPhone = 'The Primary Phone field is required.';
        }
        else if (!validatePhoneNumber(primaryPhone)) {
            result.primaryPhone = 'Enter a valid phone number in the format \'555555555\'';
        }

        if ((email !== '' && email !== undefined) && !validateEmail(email)) {
            result.email = 'Enter a valid email address.';
        }

        return new Promise(resolve => {
            this.setState({ validation: result }, () => {
                result.isValid = JSON.stringify(result) === JSON.stringify(initialState.validation);
                resolve(result);
            });                
        });
    }

    render() {
        let { classes, intent, open } = this.props;
        let { cardNumber, firstName, lastName, address, primaryPhone, email, verificationMethod, photoBase64 } = this.state.veteran;
        let validation = this.state.validation;
        let fullName = firstName + ' ' + lastName;
        let oldCardNumber = this.props.veteran ? this.props.veteran.cardNumber : '';

        let adding = this.state.addApi.isExecuting;
        let updating = this.state.updateApi.isExecuting;
        let deleting = this.state.deleteApi.isExecuting;
        
        let executing = adding || updating || deleting;

        let dim = executing ? { opacity: 0.5 } : undefined;
        
        return (
            <Dialog 
                open={open}
                onClose={this.handleCancelClick}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}
            >
                <DialogTitle style={dim}>{(intent === 'add' ? 'Enroll' : 'Update')} Veteran</DialogTitle>
                <DialogContent>
                    {!this.state.getApi.isExecuting ? 
                        photoBase64 ? <Avatar 
                            alt={fullName}
                            className={classes.photo}
                            src={photoBase64}
                        /> : <Avatar className={classes.photo}><PhotoCamera/></Avatar>
                    : <Avatar className={classes.photo}><CircularProgress size={30} color={'secondary'}/></Avatar>}
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
                    <Grid container>
                        <Grid item xs>
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
                        </Grid>
                    {isMobileAttached() &&
                        <Grid item xs={2}>
                            <Avatar className = {classes.avatar}>
                                <Button 
                                    onClick={this.handleScanClick} 
                                    className = {classes.scanButton}
                                >
                                    <SpeakerPhone/>
                                </Button>
                            </Avatar>
                        </Grid>
                    }
                    </Grid>
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
                            <MenuItem value={'Unverified'}>Unverified</MenuItem>
                            <MenuItem value={'StateIdAndDD214'}>State ID and DD214</MenuItem>
                            <MenuItem value={'IowaVeteranDL'}>Iowa Veteran's Driver's License</MenuItem>
                            <MenuItem value={'MilitaryId'}>Active Military ID</MenuItem>
                        </Select>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    {intent === 'update' && userCanView() && 
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
                    open={this.state.confirmDialog === 'delete'}
                    onConfirm={this.handleDeleteConfirmClick}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to delete Veteran '{firstName + ' ' + lastName}'?</p>
                </ConfirmDialog>
                <ConfirmDialog
                    title={'Confirm Card Number Change'}
                    prompt={'Change'}
                    open={this.state.confirmDialog === 'changeCardNumber'}
                    onConfirm={this.handleUpdateConfirmClick}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to change this Veteran's card number to {cardNumber}? The previous card, {oldCardNumber}, will no longer function.</p>
                </ConfirmDialog>
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

export default withStyles(styles)(withContext(VeteranDialog)); 