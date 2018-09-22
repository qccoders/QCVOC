/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import { Route, Switch, withRouter } from 'react-router-dom';
import PropTypes from 'prop-types';
import { getCredentials, saveLocalCredentials, saveSessionCredentials, deleteCredentials, updateCredentials } from '../credentialStore';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';
import { People, VerifiedUser, Assignment, InsertInvitation, SpeakerPhone } from '@material-ui/icons';
import Drawer from '@material-ui/core/Drawer';

import AppBar from './AppBar';
import Link from './Link';
import SecurityMenu from '../security/SecurityMenu';
import DrawerToggleButton from './DrawerToggleButton';

import Accounts from '../accounts/Accounts';
import Veterans from '../veterans/Veterans';
import Services from '../services/Services';
import Events from '../events/Events';
import LoginForm from '../security/LoginForm';
import LinkList from './LinkList';
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
    drawer: {
        width: 235,
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
                    });
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
        });
    }

    handleLogout = () => {
        return new Promise((resolve, reject) => {
            setTimeout(() => {
                this.setState({ credentials: initialState.credentials }, () => {
                    deleteCredentials();
                    this.props.history.push("/");
                });
            }, 500);
        });
    }

    handlePasswordReset = () => {
        this.setState({ 
            credentials: { ...this.state.credentials, passwordResetRequired: false }
        }, () => updateCredentials(this.state.credentials));
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
                                PaperProps={{style: styles.drawer}}
                            >
                                <AppBar title='QCVOC'/>
                                <LinkList onLinkClick={this.handleToggleDrawer}>
                                    <Link to='/veterans' icon={<People/>}>Veterans</Link>
                                    <Link to='/events' icon={<InsertInvitation/>}>Events</Link>
                                    <Link to='/scanner' icon={<SpeakerPhone/>}>Scanner</Link>
                                    {(role === 'Administrator' || role === 'Supervisor') && 
                                        <div>
                                            <ListSubheader>Administration</ListSubheader>                               
                                            <Link to='/services' icon={<Assignment/>}>Services</Link>
                                            <Link to='/accounts' icon={<VerifiedUser/>}>Accounts</Link>
                                        </div>
                                    }
                                </LinkList>                    
                            </Drawer>
                            <Switch>
                                <Route exact path='/' component={Veterans}/>
                                <Route path='/accounts' render={(props) => <Accounts {...props} onPasswordReset={this.handlePasswordReset}/>}/>
                                <Route path='/veterans' component={Veterans}/>
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

export default withRouter(withStyles(styles)(App)); 