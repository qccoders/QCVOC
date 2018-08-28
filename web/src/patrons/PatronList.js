import React from 'react';
import PropTypes from 'prop-types';
import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText, ListItemSecondaryAction, IconButton } from '@material-ui/core';
import { Person, Star, SupervisorAccount, LockOpen } from '@material-ui/icons'

const PatronList = (props) => {
    let { patrons, onItemClick } = props;

    return (
        <List>
            {patrons.sort(sortByProp('firstName')).map(p => 
                <ListItem 
                    key={p.id}
                    button
                    onClick={() => onItemClick(p)}
                >
                    <ListItemIcon>
                        <Person/>
                    </ListItemIcon>
                    <ListItemText
                        primary={p.firstName + ' ' + p.lastName}
                        secondary={p.address}
                    />
                </ListItem>
            )}
        </List>
    );
}

PatronList.propTypes = {
    patrons: PropTypes.array.isRequired,
    onItemClick: PropTypes.func.isRequired,
};

export default PatronList; 