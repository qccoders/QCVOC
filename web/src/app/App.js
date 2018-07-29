import React, { Component } from 'react';
import { Route, Switch } from 'react-router-dom';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import AppBar from './AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import InboxIcon from '@material-ui/icons/Inbox';
import Drawer from '@material-ui/core/Drawer';
import { List } from '../../node_modules/@material-ui/core';

import Link from './Link';

import Accounts from '../accounts/Accounts';
import Patrons from '../patrons/Patrons';
import Services from '../services/Services';
import Events from '../events/Events';

const styles = {
    root: {
        flexGrow: 1,
    },
};

const initialState = {
    drawer: {
        open: false,
    },
}

class App extends Component {
    state = initialState;

    toggleDrawer = () => { 
        this.setState({ drawer: { open: !this.state.drawer.open }});
    }

    render() {
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
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
                </Switch>
            </div>
        );
    }
}

App.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(App); 