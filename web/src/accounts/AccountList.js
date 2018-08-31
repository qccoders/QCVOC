/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';
import { sortByProp } from '../util';

import { List, ListItem, ListItemIcon, ListItemText, ListItemSecondaryAction, IconButton, ListSubheader } from '@material-ui/core';
import { Person, Star, SupervisorAccount, LockOpen } from '@material-ui/icons'
import AccountListItem from './AccountListItem'

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

    accounts = accounts.sort(sortByProp('name'));

    let users = accounts.filter(a => a.role === 'User');
    let supers = accounts.filter(a => a.role === 'Supervisor');
    let admins = accounts.filter(a => a.role === 'Administrator');

    return (
        <List>
            {users.length > 0 && <ListSubheader>Users</ListSubheader>}
            {users.map(a => 
                <AccountListItem 
                    key={a.id}
                    account={a}
                    onItemClick={onItemClick}
                    onItemResetClick={onItemResetClick}
                />)
            }
            {supers.length > 0 && <ListSubheader>Supervisors</ListSubheader>}
            {supers.map(a => 
                <AccountListItem 
                    key={a.id}
                    account={a}
                    onItemClick={onItemClick}
                    onItemResetClick={onItemResetClick}
                />)
            }
            {admins.length > 0 && <ListSubheader>Administrators</ListSubheader>}
            {admins.map(a => 
                <AccountListItem 
                    key={a.id}
                    account={a}
                    onItemClick={onItemClick}
                    onItemResetClick={onItemResetClick}
                />)
            }
        </List>
    );
}

AccountList.propTypes = {
    accounts: PropTypes.array.isRequired,
    onItemClick: PropTypes.func.isRequired,
    onItemResetClick: PropTypes.func.isRequired,
};

export default AccountList; 