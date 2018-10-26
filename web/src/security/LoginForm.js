/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { 
    Card, 
    CardContent, 
    CardActions, 
    CircularProgress,
    TextField,
    Button,
    FormControlLabel,
    Checkbox,
} from '@material-ui/core';

import { withContext } from '../shared/ContextProvider';
import logo from '../assets/qcvo.png';

const styles = {
    root: {
        flexGrow: 1,
        position: 'absolute',
        top: 0,
        bottom: 0,
        left: 0,
        right: 0,
        backgroundColor: '#3f51b5',
        display: 'grid',
        textAlign: 'center',
        padding: 20,
        minHeight: 470,
    },
    card: {
        minWidth: 180,
        maxWidth: 400,
        margin: 'auto',
        marginTop: 0,
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
};

class LoginForm extends Component {
    state = initialState;

    nameInput = React.createRef();
    passwordInput = React.createRef();
    loginButton = React.createRef();

    componentDidMount = () => {
        this.nameInput.focus();
    }

    handleChange = (field, value) => {
        this.setState({ 
            ...this.state, 
            [field]: value,
        });
    }

    handleLoginClick = () => {
        this.setState({ 
            api: { isExecuting: true },
            
        }, () => {
            this.props.context.api.post('/v1/security/login', this.state)
            .then(
                response => {
                    this.props.onLogin(response.data, this.state.rememberMe);
                }, 
                error => {
                    this.setState({ api: { isExecuting: false, isErrored: true }}, () => {
                        if (error.response
                            && (error.response.status === 400 || error.response.status === 401)) {
                            this.setState({ password: '' }, () => {
                                this.passwordInput.focus();
                            });
                        }
                    });
                }
            );
        });
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
                            margin="normal"
                            disabled={isExecuting}
                            onChange={(event) => this.handleChange('name', event.target.value)}
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
                            onChange={(event) => this.handleChange('password', event.target.value)}
                            inputRef={ref => this.passwordInput = ref}
                        />
                        <FormControlLabel
                            className={classes.checkbox}
                            label="Remember Me"
                            disabled={isExecuting}
                            control={
                                <Checkbox
                                    checked={this.state.rememberMe}
                                    onChange={(event) => this.handleChange('rememberMe', !this.state.rememberMe)}
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
            </div>
        );
    }
}

LoginForm.propTypes = {
    onLogin: PropTypes.func.isRequired,
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(withContext(LoginForm)); 