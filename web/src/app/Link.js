import React, { Component } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { ListItem, ListItemIcon, ListItemText } from '../../node_modules/@material-ui/core';

const styles = {

};

class Link extends Component {
    render() {
        let classes = this.props.classes;

        return (
            <RouterLink to={this.props.to}>
                <ListItem button>
                    <ListItemIcon>{this.props.icon}</ListItemIcon>
                    <ListItemText primary={this.props.children}/>
                </ListItem>
            </RouterLink>
        );
    }
}

Link.propTypes = {
  classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Link); 