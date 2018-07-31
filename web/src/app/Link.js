import React from 'react';
import { Link as RouterLink } from 'react-router-dom';
import PropTypes from 'prop-types';

import { ListItem, ListItemIcon, ListItemText } from '@material-ui/core';

const Link = (props) => {
    return (
        <ListItem 
            button 
            component={RouterLink} 
            to={props.to}
        >            
            <ListItemIcon>{props.icon}</ListItemIcon>
            <ListItemText primary={props.children}/>
        </ListItem>
    );
}

Link.propTypes = {
  icon: PropTypes.object.isRequired,
  children: PropTypes.string.isRequired,
  to: PropTypes.string.isRequired,
};

export default Link; 