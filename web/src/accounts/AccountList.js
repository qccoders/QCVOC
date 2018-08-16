import React from 'react';
import PropTypes from 'prop-types';
import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText, } from '@material-ui/core';
import { Person, Star, SupervisorAccount, } from '@material-ui/icons'

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
    let { accounts, onItemClick } = props;

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