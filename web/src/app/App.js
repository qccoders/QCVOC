/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import { Route, Switch } from 'react-router-dom';
import PropTypes from 'prop-types';
import { getCredentials, saveLocalCredentials, saveSessionCredentials, deleteCredentials } from '../credentialStore';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';
import { People, Inbox, VerifiedUser, AssignmentTurnedIn, InsertInvitation } from '@material-ui/icons';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';

import AppBar from './AppBar';
import Link from './Link';
import SecurityMenu from '../security/SecurityMenu';
import DrawerToggleButton from './DrawerToggleButton';

import Accounts from '../accounts/Accounts';
import Patrons from '../patrons/Patrons';
import Services from '../services/Services';
import Events from '../events/Events';
import LoginForm from '../security/LoginForm';
import { CircularProgress, ListSubheader } from '@material-ui/core';

const styles = {
    root: {
        flexGrow: 1,
    },
    spinner: {
        position: 'fixed',
        width: 45,
        height: 45,
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 'auto',
        marginBottom: 'auto',
    },
};

const initialState = {
    api: {
        isExecuting: false,
        isErrored: false,
    },
    credentials: {
        accessToken: undefined,
        refreshToken: undefined,
        expires: undefined,
        issued: undefined,
        tokenType: undefined,
    },
    drawer: {
        open: false,
    },
}

class App extends Component {
    state = initialState;

    componentDidMount = () => {
        if (getCredentials()) {
            this.setState({ api: { ...this.state.api, isExecuting: true }}, () => {
                api.get('/v1/security').then(() => {
                    this.setState({ 
                        api: { isExecuting: false, isErrored: false },
                        credentials: getCredentials() 
                    }, () => this.getAccountDetails());
                })
                .catch(error => {
                    this.setState({ 
                        credentials: initialState.credentials,
                        api: { isExecuting: false, isErrored: true }
                    });
                });
            })
        }
    }

    getAccountDetails = () => {
        let { credentials } = this.state;

        api.get('/v1/security/accounts/' + credentials.id)
        .then(response => {
            this.setState({ credentials: { ...credentials, passwordResetRequired: response.data.passwordResetRequired }});
        })
        .catch(error => {
            console.log(error);
        });
    }

    handleToggleDrawer = () => { 
        this.setState({ drawer: { open: !this.state.drawer.open }});
    }

    handleLogin = (credentials, persistCredentials) => {
        this.setState({ credentials: credentials }, () => {
            if (persistCredentials) {
                saveLocalCredentials(this.state.credentials);
            } else {
                saveSessionCredentials(this.state.credentials);
            }
            this.getAccountDetails();
        });
    }

    handleLogout = () => {
        return new Promise((resolve, reject) => {
            setTimeout(() => {
                this.setState({ credentials: initialState.credentials }, () => {
                    deleteCredentials();
                });
            }, 500);
        });
    }

    handlePasswordReset = () => {
        this.getAccountDetails();
    }

    render() {
        let classes = this.props.classes;
        let { isExecuting, isErrored } = this.state.api;
        let { accessToken, role } = this.state.credentials;

        return (
            <div className={classes.root}>
                {isExecuting || isErrored ? <CircularProgress size={20} style={styles.spinner}/>: 
                    accessToken ? 
                        <div>
                            <AppBar 
                                title='QCVOC' 
                                drawerToggleButton={<DrawerToggleButton onToggleClick={this.handleToggleDrawer}/>}
                            >
                                <SecurityMenu 
                                    credentials={this.state.credentials} 
                                    onLogout={this.handleLogout}
                                    onPasswordReset={this.handlePasswordReset}
                                />
                            </AppBar>
                            <Drawer 
                                open={this.state.drawer.open} 
                                onClose={this.handleToggleDrawer}
                            >
                                <AppBar title='QCVOC'/>
                                <List>
                                    <Link to='/patrons' icon={<People/>}>Patrons</Link>
                                    <Link to='/events' icon={<InsertInvitation/>}>Events</Link>
                                    {(role === 'Administrator' || role === 'Supervisor') && 
                                        <div>
                                            <ListSubheader>Administration</ListSubheader>                               
                                            <Link to='/services' icon={<AssignmentTurnedIn/>}>Services</Link>
                                            <Link to='/accounts' icon={<VerifiedUser/>}>Accounts</Link>
                                        </div>
                                    }
                                </List>                    
                            </Drawer>
                            <Switch>
                                <Route path='/accounts' component={Accounts}/>
                                <Route path='/patrons' component={Patrons}/>
                                <Route path='/services' component={Services}/>
                                <Route path='/events' component={Events}/>
                            </Switch>
                        </div> :
                        <LoginForm onLogin={this.handleLogin}/>
                }
            </div>
        );
    }
}

App.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(App); 