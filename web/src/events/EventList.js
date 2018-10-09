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
        let start = moment(event.startDate);
        let end = moment(event.endDate);

        let startFmt = 'dddd, MMMM Do [from] LT';
        let endFmt = '';

        if (start.format('L') !== end.format('L')) {
            endFmt = 'LT [on] dddd, MMMM Do';
        }
        else {
            endFmt = 'LT';
        }

        return start.local().format(startFmt) + ' to ' + end.local().format(endFmt);
    }

    return (
        <List>
            {events.sort(sortByProp('startDate')).map((e, index) => 
                <ListItem 
                    key={index}
                    button={onItemClick !== undefined}
                    onClick={onItemClick !== undefined ? () => onItemClick(e) : () => {}}
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
    onItemClick: PropTypes.func,
};

export default EventList; 