import React from 'react';
import PropTypes from 'prop-types';
import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText, ListItemSecondaryAction, IconButton } from '@material-ui/core';
import { Person, Star, SupervisorAccount, Delete } from '@material-ui/icons'

const getUserIcon = (role) => {
    switch (role) {
        case 'Administrator':
            return <Star/>;
        case 'Supervisor':
            return <SupervisorAccount/>;
        default:
            return <Person/>;
    }
}

const AccountList = (props) => {
    let { accounts, onItemClick, onItemDeleteClick } = props;

    return (
        <List>
            {accounts.sort(sortByProp('name')).map(a => 
                <ListItem 
                    key={a.id}
                    button
                    onClick={() => onItemClick(a)}
                >
                    <ListItemIcon>
                        {getUserIcon(a.role)}
                    </ListItemIcon>
                    <ListItemText
                        primary={a.name}
                        secondary={a.role}
                    />
                    <ListItemSecondaryAction>
                        <IconButton onClick={() => onItemDeleteClick(a)}>
                            <Delete/>
                        </IconButton>
                    </ListItemSecondaryAction>
                </ListItem>
            )}
        </List>
    );
}

AccountList.propTypes = {
    accounts: PropTypes.array.isRequired,
    onItemClick: PropTypes.func.isRequired,
};

export default AccountList; 