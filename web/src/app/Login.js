import React, { Component } from 'react';
import PropTypes from 'prop-types';
import axios from 'axios';

import { API_ROOT } from '../constants';
import { withStyles } from '@material-ui/core/styles';

import { Card, CardContent, CardActions } from '@material-ui/core';
import TextField from '@material-ui/core/TextField';
import Button from '@material-ui/core/Button';

const styles = {
    root: {
        flexGrow: 1,
    },
    card: {
        width: 400,
        margin: 'auto',
        marginTop: 30,
    },
};

class Login extends Component {
    state = { name: '', password: '' }

    handleChange = (field, event) => {
        this.setState({ [field]: event.target.value });
    }

    handleLoginClick = () => {
        axios.post(API_ROOT + '/v1/tokens', this.state)
        .then(
            response => this.props.onLogin(response.data), 
            error => console.log(error.response)
        );
    }

    render() {
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
                <Card className={classes.card}>
                    <CardContent>
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
                    </CardContent>
                    <CardActions>
                        <Button size="small" onClick={this.handleLoginClick}>Log In</Button>
                    </CardActions>
                </Card>
            </div>
        );
    }
}

Login.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Login); 