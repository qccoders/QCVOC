/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';

import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';

const EventList = (props) => {
    let { events, icon, onItemClick } = props;

    // todo: format date with 'from' = a time if the same date as the start, include end date if not 

    return (
        <List>
            {events.sort(sortByProp('startDate')).map(e => 
                <ListItem 
                    key={e.id}
                    button
                    onClick={() => onItemClick(e)}
                >
                    <ListItemIcon>
                        {icon}
                    </ListItemIcon>
                    <ListItemText
                        primary={e.name}
                        secondary={moment(e.startDate).format('dddd, MMMM Do') + ' from ' + moment(e.startDate).format('h:mm a') + ' to ' + moment(e.endDate).format('h:mm a')}
                    />
                </ListItem>
            )}
        </List>
    );
}

EventList.propTypes = {
    events: PropTypes.array.isRequired,
    icon: PropTypes.object.isRequired,
    onItemClick: PropTypes.func.isRequired,
};

export default EventList; 