import React from 'react';
import { withStyles } from '@material-ui/core/styles';
import PropTypes from 'prop-types';
import { AppBar as MaterialAppBar } from '@material-ui/core';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';

const styles = {
    menuButton: {
        marginLeft: -12,
        marginRight: 20,
    },
};

const AppBar = (props) => {
    return (
        <MaterialAppBar position="static" color="primary">
            <Toolbar>
                {!props.menu ? '' : 
                    <IconButton 
                        className={props.classes.menuButton} 
                        color="inherit" 
                        onClick={props.onMenuClick}
                    >
                        <MenuIcon/>
                    </IconButton>
                }
                <Typography variant="title" color="inherit">
                    {props.title}
                </Typography>
            </Toolbar>
        </MaterialAppBar>
    );
}

AppBar.propTypes = {
    classes: PropTypes.object.isRequired,
    onMenuClick: PropTypes.func,
    title: PropTypes.string,
    menu: PropTypes.bool,
};

export default withStyles(styles)(AppBar); 