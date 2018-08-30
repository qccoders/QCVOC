/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
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
        name: undefined,
        role: undefined,
        password: undefined,
        password2: undefined,
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

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleSaveClick = () => {
        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => this.props.addAccount({ ...this.state.account }),
                        'addApi', 
                        'Account \'' + this.state.account.name + '\' successfully created.'
                    )
                }
                else {
                    this.execute(
                        () => this.props.updateAccount({ ...this.state.account }), 
                        'updateApi', 
                        'Account \'' + this.state.account.name + '\' successfully updated.'
                    );
                }
            }
        });
    }

    handleDeleteClick = () => {
        this.setState({ confirmDialog: { open: true }});
    }

    handleDeleteConfirmation = () => {
        return this.execute(
            () => this.props.deleteAccount({ ...this.state.account }), 
            'deleteApi', 
            'Account \'' + this.state.account.name + '\' successfully deleted.'
        );
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
                action(this.state.patron)
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
        let { name, role, password, password2 } = this.state.patron;
        let result = { ...initialState.validation };

        if (name === '') {
            result.name = 'The Name field is required.';
        }

        if (role === '') {
            result.role = 'Select a Role.';
        }

        if (this.props.intent === 'add') {
            if (password === '') {
                result.password = 'The Password field is required.';
            }

            if (password2 === '') {
                result.password2 = 'The Confirm Password field is required.';
            }

            if (password !== '' && password2 !== '' && password !== password2) {
                result.password = result.password2 = 'The Password fields must match.';
            }
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
                onClose={this.handleCancel}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}
            >
                <DialogTitle>{(intent === 'add' ? 'Add' : 'Update')} Patron</DialogTitle>
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
                    title={'Confirm Patron Deletion'}
                    prompt={'Delete'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleDeleteConfirmation}
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