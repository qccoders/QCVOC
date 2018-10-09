/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';
import { withContext } from '../shared/ServiceContext';

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
import ConfirmDialog from '../shared/ConfirmDialog';


const styles = {
    dialog: {
        width: 320,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
    deleteButton: {
        marginRight: 'auto',
    },
    roleSelect: {
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
    account: {
        name: '',
        role: 'User',
        password: '',
        password2: '',
    },
    validation: {
        name: undefined,
        role: undefined,
        password: undefined,
        password2: undefined,
    },
    confirmDialog: {
        open: false,
    },
}

class AccountDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState, 
                account: nextProps.account ? nextProps.account : { 
                    ...initialState.account, 
                },
                validation: initialState.validation,
            });
        }
    }

    handleChange = (field, event) => {
        this.setState({ 
            account: {
                ...this.state.account,
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
        let account = this.state.account;

        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => api.post('/v1/security/accounts', account),
                        'addApi', 
                        'Account \'' + account.name + '\' successfully created.'
                    )
                }
                else {
                    this.execute(
                        () => api.patch('/v1/security/accounts/' + account.id, account), 
                        'updateApi', 
                        'Account \'' + account.name + '\' successfully updated.'
                    );
                }
            }
        });
    }

    handleDeleteClick = () => {
        this.setState({ confirmDialog: { open: true }});
    }

    handleDeleteConfirmation = () => {
        let account = this.state.account;

        return this.execute(
            () => api.delete('/v1/security/accounts/' + account.id), 
            'deleteApi', 
            'Account \'' + account.name + '\' successfully deleted.'
        );
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    execute = (action, api, successMessage) => {
        return new Promise((resolve, reject) => {
            this.setState({ [api]: { isExecuting: true }}, () => {
                this.props.context.apiCall(action, this.state.account)
                .then(response => {
                    this.setState({
                        [api]: { isExecuting: false, isErrored: false }
                    }, () => {
                        this.props.onClose(successMessage);
                        resolve(response);
                    })
                }, error => {
                    this.setState({ 
                        [api]: { isExecuting: false, isErrored: true }
                    }, () => reject(error));
                })
            })
        })
    }

    validate = () => {
        let { name, role, password, password2 } = this.state.account;
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
        let { name, role } = this.state.account;
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
                <DialogTitle>{(intent === 'add' ? 'Add' : 'Update')} Account</DialogTitle>
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
                    />
                    <FormControl 
                        className={classes.roleSelect}
                        fullWidth
                        disabled={executing}
                    >
                        <InputLabel>Role</InputLabel>
                        <Select
                            value={role}
                            onChange={(event) => this.handleChange('role', event)}
                            fullWidth
                        >
                            <MenuItem value={'User'}>User</MenuItem>
                            <MenuItem value={'Supervisor'}>Supervisor</MenuItem>
                            <MenuItem value={'Administrator'}>Administrator</MenuItem>
                        </Select>
                    </FormControl>
                    {intent !== 'add' ? '' : 
                        <div>
                            <TextField
                                style={{marginTop: 30}}
                                id="password"
                                label="Password"
                                type="password"
                                error={validation.password !== undefined}
                                helperText={validation.password}
                                fullWidth
                                onChange={(event) => this.handleChange('password', event)}
                                disabled={executing}
                            />
                            <TextField
                                style={{marginTop: 15}}
                                id="password2"
                                label="Confirm Password"
                                type="password"
                                error={validation.password2 !== undefined}
                                helperText={validation.password2}
                                fullWidth
                                onChange={(event) => this.handleChange('password2', event)}
                                disabled={executing}
                            />
                        </div>
                    }
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
                    title={'Confirm Account Deletion'}
                    prompt={'Delete'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleDeleteConfirmation}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to delete account '{this.state.account.name}'?</p>
                </ConfirmDialog>
            </Dialog>
        );
    }
}

AccountDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'update' ]).isRequired,
    open: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    account: PropTypes.object,
};

export default withContext(withStyles(styles)(AccountDialog)); 