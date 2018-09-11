/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';

import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { InsertInvitation } from '@material-ui/icons'

const EventList = (props) => {
    let { events, onItemClick } = props;

    return (
        <List>
            {events.sort(sortByProp('startDate')).map(e => 
                <ListItem 
                    key={e.id}
                    button
                    onClick={() => onItemClick(e)}
                >
                    <ListItemIcon>
                        <InsertInvitation/>
                    </ListItemIcon>
                    <ListItemText
                        primary={e.name}
                        secondary={moment(e.startDate).format('LLLL') + ' to ' + moment(e.endDate).format('h:mm a')}
                    />
                </ListItem>
            )}
        </List>
    );
}

EventList.propTypes = {
    events: PropTypes.array.isRequired,
    onItemClick: PropTypes.func.isRequired,
};

export default EventList; 