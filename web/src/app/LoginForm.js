import React, { Component } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';

import { API_ROOT } from '../constants';
import { withStyles } from '@material-ui/core/styles';

import { Card, CardContent, CardActions } from '@material-ui/core';
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
};

class LoginForm extends Component {
    state = { name: '', password: '', rememberMe: true }

    handleChange = (field, event) => {
        this.setState({ [field]: event.target.value });
    }

    handleLoginClick = () => {
        axios.post(API_ROOT + '/v1/tokens', this.state)
        .then(
            response => this.props.onLogin(response.data, this.state.rememberMe), 
            error => console.log(error.response)
        );
    }

    render() {
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
                <Card className={classes.card}>
                    <CardContent>
                        <img className={classes.logo} src={logo} alt="logo"/>
                        <TextField
                            id="name"
                            label="Name"
                            className={classes.textField}
                            value={this.state.name}
                            margin="normal"
                            onChange={(event) => this.handleChange('name', event)}
                        />
                        <TextField
                            id="password-input"
                            label="Password"
                            className={classes.textField}
                            type="password"
                            autoComplete="current-password"
                            margin="normal"
                            onChange={(event) => this.handleChange('password', event)}
                        />
                        <FormControlLabel
                            className={classes.checkbox}
                            label="Remember Me"
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
                        >
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