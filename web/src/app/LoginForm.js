import React, { Component } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';

import { API_ROOT } from '../constants';
import { withStyles } from '@material-ui/core/styles';

import { Card, CardContent, CardActions, CircularProgress } from '@material-ui/core';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';
import FormControlLabel from '@material-ui/core/FormControlLabel';
import Checkbox from '@material-ui/core/Checkbox';
import Snackbar from '@material-ui/core/Snackbar';

import logo from '../assets/qcvo.png';

const styles = {
    root: {
        flexGrow: 1,
        backgroundColor: '#3f51b5',
        display: 'grid',
        height: '100vh',
        textAlign: 'center',
    },
    card: {
        minWidth: 180,
        maxWidth: 400,
        margin: 'auto',
        marginTop: 30,
    },
    textField: {
        width: '100%',
    },
    button: {
        width: '100%',
        margin: 10,
    },
    checkbox:{
        marginTop: 10,
        width: '100%',
    },
    logo: {
        maxWidth: '100%',
        maxHeight: '100%',
    },
    spinner: {
        position: 'fixed',
    },
};

const initialState = {
    name: '',
    password: '',
    rememberMe: true,
    api: {
        isExecuting: false,
        isErrored: false,
    },
    validation: {
        name: undefined,
        password: undefined,
    },
    snackbar: {
        message: '',
        open: false,
    },
}

class LoginForm extends Component {
    state = initialState;

    nameInput = React.createRef();
    passwordInput = React.createRef();
    loginButton = React.createRef();

    componentDidMount = () => {
        this.nameInput.focus();
    }

    handleChange = (field, event) => {
        this.setState({ 
            ...this.state, 
            [field]: event.target.value,
            validation: {
                ...this.state.validation,
                [field]: undefined,
            },
        });
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    handleLoginClick = () => {
        this.setState({ 
            api: { isExecuting: true },
            validation: { name: undefined, password: undefined },
            
        }, () => {
            axios.post(API_ROOT + '/v1/tokens', this.state)
            .then(
                response => {
                    this.props.onLogin(response.data, this.state.rememberMe);
                }, 
                error => {
                    this.setState({ api: { isExecuting: false, isErrored: true }}, () => {
                        if (error.response && error.response.status === 400) {
                            if (error.response.data) {
                                let validation = this.state.validation;

                                if (error.response.data.Password && error.response.data.Password.length > 0) {
                                    validation.password = error.response.data.Password[0];
                                    this.passwordInput.focus();
                                }
                                if (error.response.data.Name && error.response.data.Name.length > 0) {
                                    validation.name = error.response.data.Name[0];
                                    this.nameInput.focus();
                                }

                                this.setState({ validation: validation });
                            }
                        }
                        else if (error.response && error.response.status === 401) {
                            this.setState({ 
                                password: '', 
                                snackbar: { 
                                    message: 'Login failed.',
                                    open: true,
                                }
                            }, () => {
                                this.passwordInput.focus();
                            });
                        }
                        else {
                            console.log(error)
                            this.setState({ 
                                snackbar: { 
                                    message: 'Error: ' + error.message,
                                    open: true 
                                }
                            });
                        }
                    });
                }
            );
        })
    }

    handleKeyPress = (event) => {
        if (event.charCode === 13) {
            this.loginButton.click();
        }
    }

    render() {
        let classes = this.props.classes;
        let isExecuting = this.state.api.isExecuting;

        return (
            <div className={classes.root}>
                <Card className={classes.card} onKeyPress={(event) => this.handleKeyPress(event)}>
                    <CardContent>
                        <img className={classes.logo} src={logo} alt="logo" style={isExecuting ? {filter: 'grayscale(100%)', opacity: 0.5} : {}}/>
                        <TextField
                            id="name"
                            label="Name"
                            className={classes.textField}
                            value={this.state.name}
                            error={this.state.validation.name !== undefined}
                            helperText={this.state.validation.name}
                            margin="normal"
                            disabled={isExecuting}
                            onChange={(event) => this.handleChange('name', event)}
                            inputRef={ref => this.nameInput = ref}
                        />
                        <TextField
                            id="password-input"
                            label="Password"
                            className={classes.textField}
                            value={this.state.password}
                            type="password"
                            autoComplete="current-password"
                            margin="normal"
                            disabled={isExecuting}
                            error={this.state.validation.password !== undefined}
                            helperText={this.state.validation.password}
                            onChange={(event) => this.handleChange('password', event)}
                            inputRef={ref => this.passwordInput = ref}
                        />
                        <FormControlLabel
                            className={classes.checkbox}
                            label="Remember Me"
                            disabled={isExecuting}
                            control={
                                <Checkbox
                                    checked={this.state.rememberMe}
                                    onChange={(event) => this.handleChange('rememberMe', event)}
                                    color="primary"
                                />
                            }
                        />
                    </CardContent>
                    <CardActions>
                        <Button 
                            variant='contained'
                            color='secondary'
                            className={classes.button}
                            onClick={this.handleLoginClick}
                            disabled={isExecuting}
                            buttonRef={ref => this.loginButton = ref}
                        >
                            {isExecuting && <CircularProgress size={20} style={styles.spinner}/>}
                            Log In
                        </Button>
                    </CardActions>
                </Card>
                <Snackbar
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'center'}}
                    open={this.state.snackbar.open}
                    onClose={this.handleSnackbarClose}
                    autoHideDuration={3000}
                    message={<span id="message-id">{this.state.snackbar.message}</span>}
                />
            </div>
        );
    }
}

LoginForm.propTypes = {
    onLogin: PropTypes.func.isRequired,
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(LoginForm); 