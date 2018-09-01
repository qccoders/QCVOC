/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

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
            onClick={props.onClick}
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