/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';
import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText, ListItemSecondaryAction, IconButton } from '@material-ui/core';
import { Person, Star, SupervisorAccount, LockOpen } from '@material-ui/icons'

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
    let { accounts, onItemClick, onItemResetClick } = props;

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
                        <IconButton onClick={() => onItemResetClick(a)}>
                            <LockOpen/>
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