import React, { Component } from 'react';
import { Route, Switch } from 'react-router-dom';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import InboxIcon from '@material-ui/icons/Inbox';
import Drawer from '@material-ui/core/Drawer';
import { List, ListItem, ListItemIcon, ListItemText } from '../../node_modules/@material-ui/core';

import Accounts from '../accounts/Accounts';

const styles = {
    root: {
        flexGrow: 1,
    },
    flex: {
        flexGrow: 1,
    },
    menuButton: {
        marginLeft: -12,
        marginRight: 20,
    },
    drawer: {
        width: 500,
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
                <AppBar position="static" color="primary">
                    <Toolbar>
                        <IconButton 
                            className={classes.menuButton} 
                            color="inherit" 
                            aria-label="Menu"
                            onClick={this.toggleDrawer}
                        >
                            <MenuIcon/>
                        </IconButton>
                        <Typography variant="title" color="inherit">
                            QCVOC
                        </Typography>
                    </Toolbar>
                </AppBar>
                <Drawer 
                    open={this.state.drawer.open} 
                    onClose={this.toggleDrawer}
                    className={classes.drawer}
                >
                    <AppBar position="static" color="primary">
                        <Toolbar>
                            <Typography variant="title" color="inherit">
                                QCVOC
                            </Typography>
                        </Toolbar>
                    </AppBar>
                    <List>
                        <ListItem button>
                            <ListItemIcon><InboxIcon/></ListItemIcon>
                            <ListItemText primary="Testas aasdfasdf asdfasd "/>
                        </ListItem>
                    </List>                    
                </Drawer>
                <Switch>
                    <Route exact path='/accounts' component={Accounts}/>
                    <Route exact path='/patrons' component={Patrons}/>
                </Switch>
            </div>
        );
    }
}

App.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(App); 