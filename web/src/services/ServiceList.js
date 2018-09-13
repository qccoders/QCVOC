/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import PropTypes from 'prop-types';

import { List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { Shop } from '@material-ui/icons'

const ServiceList = (props) => {
    let { services, onItemClick } = props;

    return (
        <List>
            {services.map(s =>
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