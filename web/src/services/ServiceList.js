/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import PropTypes from 'prop-types';

import { List, ListItem, ListItemIcon, ListItemText, ListSubheader } from '@material-ui/core';
import { Shop } from '@material-ui/icons'

const ServiceList = (props) => {
    let { services, onItemClick } = props;
    let userDefined = services.filter(s => s.id !== '00000000-0000-0000-0000-000000000000');
    let systemDefined = services.filter(s => s.id === '00000000-0000-0000-0000-000000000000');

    return (
        <List>
            <ListSubheader>User Defined</ListSubheader>
            {userDefined.map(s =>
                <ListItem
                    key={s.id}
                    button
                    onClick={() => onItemClick(s)}
                >
                    <ListItemIcon>
                        <Shop/>
                    </ListItemIcon>
                    <ListItemText
                        primary={s.name}
                        secondary={s.description}
                    />
                </ListItem>
            )}
            <ListSubheader>System Defined</ListSubheader>
            {systemDefined.map(s =>
                <ListItem
                    key={s.id}
                    button
                    onClick={() => onItemClick(s)}
                >
                    <ListItemIcon>
                        <Shop/>
                    </ListItemIcon>
                    <ListItemText
                        primary={s.name}
                        secondary={s.description}
                    />
                </ListItem>
            )}
        </List>
    );
}

ServiceList.propTypes = {
    services: PropTypes.array.isRequired,
    onItemClick: PropTypes.func.isRequired,
};

export default ServiceList;