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
    const { events, icon, onItemClick } = props;

    const getDate = (event) => {
        let start = 'dddd, MMMM Do [from] LT';
        let end = '';

        if (moment(event.startDate).format('L') != moment(event.endDate).format('L')) {
            end = 'LT [on] dddd, MMMM Do';
        }
        else {
            end = 'LT';
        }

        return moment(event.startDate).format(start) + ' to ' + moment(event.endDate).format(end);
    }

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
                        secondary={getDate(e)}
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