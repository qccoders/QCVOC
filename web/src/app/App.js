import React, { Component } from 'react';
import { Route, Switch } from 'react-router-dom';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import InboxIcon from '@material-ui/icons/Inbox';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';

import AppBar from './AppBar';
import Link from './Link';

import Accounts from '../accounts/Accounts';
import Patrons from '../patrons/Patrons';
import Services from '../services/Services';
import Events from '../events/Events';
import Login from './Login';

const styles = {
    root: {
        flexGrow: 1,
    },
};

const initialState = {
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

    toggleDrawer = () => { 
        this.setState({ drawer: { open: !this.state.drawer.open }});
    }

    handleLogin = (credentials) => {
        this.setState({ credentials: credentials });
    }

    render() {
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
                {this.state.credentials.accessToken ? 
                <div>
                <AppBar title='QCVOC' menu onMenuClick={this.toggleDrawer}/>
                <Drawer 
                    open={this.state.drawer.open} 
                    onClose={this.toggleDrawer}
                >
                    <AppBar title='QCVOC'/>
                    <List>
                        <Link to='/accounts' icon={<InboxIcon/>}>Accounts</Link>
                        <Link to='/patrons' icon={<InboxIcon/>}>Patrons</Link>
                        <Link to='/services' icon={<InboxIcon/>}>Services</Link>
                        <Link to='/events' icon={<InboxIcon/>}>Events</Link>
                    </List>                    
                </Drawer>
                <Switch>
                    <Route path='/accounts' component={Accounts}/>
                    <Route path='/patrons' component={Patrons}/>
                    <Route path='/services' component={Services}/>
                    <Route path='/events' component={Events}/>
                </Switch></div> :
                <Login onLogin={this.handleLogin}/>
                }
            </div>
        );
    }
}

App.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(App); 