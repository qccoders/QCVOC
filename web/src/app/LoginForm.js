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
        width: 400,
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
        width: 339,
        height: 158,
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
    }
}

class LoginForm extends Component {
    state = initialState;

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
                    console.log(error.response)

                    this.setState({ api: { isExecuting: false, isErrored: true }}, () => {
                        if (error.response.data) {
                            let validation = { ... this.state.validation };

                            if (error.response.data.Name && error.response.data.Name.length > 0) {
                                validation.name = error.response.data.Name[0];
                            }
                            if (error.response.data.Password && error.response.data.Password.length > 0) {
                                validation.password = error.response.data.Password[0];
                            }

                            this.setState({ ...this.state, validation: validation });
                        }
                    })
                }
            );
        })
    }

    render() {
        let classes = this.props.classes;
        let isExecuting = this.state.api.isExecuting;

        console.log(this.state);
        return (
            <div className={classes.root}>
                <Card className={classes.card}>
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
                        />
                        <TextField
                            id="password-input"
                            label="Password"
                            className={classes.textField}
                            type="password"
                            autoComplete="current-password"
                            margin="normal"
                            disabled={isExecuting}
                            error={this.state.validation.password !== undefined}
                            helperText={this.state.validation.password}
                            onChange={(event) => this.handleChange('password', event)}
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
                        >
                            {isExecuting ? <CircularProgress size={20} style={styles.spinner}/> : ''}
                            Log In
                        </Button>
                    </CardActions>
                </Card>
            </div>
        );
    }
}

LoginForm.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(LoginForm); 