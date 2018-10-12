/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import PropTypes from 'prop-types';

import { List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { sortByProp } from '../util';

const ServiceList = (props) => {
    let { services, onItemClick, icon } = props;
    let clickable = onItemClick !== undefined;

    return (
        <List>
            {services.sort(sortByProp('name')).map((s, index) =>
                <ListItem
                    key={index}
                    button={clickable}
                    onClick={clickable ? () => onItemClick(s) : undefined}
                >
                    <ListItemIcon>
                        {icon}
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
    onItemClick: PropTypes.func,
    icon: PropTypes.object.isRequired,
};

export default ServiceList;