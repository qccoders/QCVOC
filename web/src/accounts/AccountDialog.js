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

const styles = {
    dialog: {
        width: 320,
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
    saveApi: {
        isExecuting: false,
        isErrored: false,
    },
    deleteApi: {
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
    snackbar: {
        message: '',
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

    handleCancel = () => {
        this.props.onClose();
    }

    handleSave = () => {
        this.validate().then(result => {
            if (result.isValid) {
                this.setState({ saveApi: { isExecuting: true }}, () => {
                    this.props.addAccount(this.state.account)
                    .then(response => {
                        var name = response.data.name;

                        this.setState({ 
                            saveApi: { isExecuting: false, isErrored: false },
                        }, () => this.props.onClose('Account \'' + name + '\' successfully created.'))
                    }, error => {
                        var body = error && error.response && error.response.data ? error.response.data : error;

                        this.setState({ 
                            saveApi: { isExecuting: false, isErrored: true },
                            snackbar: {
                                message: body[Object.keys(body)[0]],
                                open: true,
                            },
                        });
                    })
                })
            }
        });
    }

    handleDelete = () => {
        this.setState({ deleteApi: { isExecuting: true }}, () => {
            this.props.deleteAccount(this.state.account.id)
            .then(response => {
                this.setState({
                    deleteApi: { isExecuting: false, isErrored: false }
                }, () => this.props.onClose('Account \'' + this.state.account.name + '\' successfully deleted.'))
            }, error => {
                var body = error && error.response && error.response.data ? error.response.data : error;

                this.setState({ 
                    deleteApi: { isExecuting: false, isErrored: true },
                    snackbar: {
                        message: body,
                        open: true,
                    },
                });
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

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    render() {
        let { classes, intent, open } = this.props;
        let { name, role } = this.state.account;
        let validation = this.state.validation;
        
        return (
            <Dialog 
                open={open}
                onClose={this.handleCancel}
                PaperProps={{ className: classes.dialog }}
            >
                <DialogTitle>{(intent === 'add' ? 'Add' : 'Edit')} Account</DialogTitle>
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
                    />
                    <FormControl 
                        className={classes.roleSelect}
                        fullWidth
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
                            />
                        </div>
                    }
                </DialogContent>
                <DialogActions>
                    {intent === 'edit' && 
                        <Button 
                            onClick={this.handleDelete} 
                            color="primary" 
                            className={classes.deleteButton}
                        >
                            {this.state.deleteApi.isExecuting && <CircularProgress size={20} style={styles.spinner}/>}
                            Delete
                        </Button>
                    }
                    <Button onClick={this.handleCancel} color="primary">Cancel</Button>
                    <Button onClick={this.handleSave} color="primary">
                        {this.state.saveApi.isExecuting && <CircularProgress size={20} style={styles.spinner}/>}
                        Save
                    </Button>
                </DialogActions>
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

AccountDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'edit' ]).isRequired,
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired,
    account: PropTypes.object,
    addAccount: PropTypes.func.isRequired,
    deleteAccount: PropTypes.func.isRequired,
};

export default withStyles(styles)(AccountDialog); 