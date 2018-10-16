/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React from 'react';
import PropTypes from 'prop-types';

import { 
    ListItem, 
    ListItemIcon, 
    ListItemText, 
    ListItemSecondaryAction, 
    IconButton,
} from '@material-ui/core';
import { Person, Star, SupervisorAccount, LockOpen } from '@material-ui/icons';

const getUserIcon = (role) => {
    switch (role) {
        case 'Administrator':
            return <Star/>;
        case 'Supervisor':
            return <SupervisorAccount/>;
        default:
            return <Person/>;
    }
};

const AccountListItem = (props) => {
    let { account, onItemClick, onItemResetClick } = props;

    return (
        <ListItem 
            key={account.id}
            button
            onClick={() => onItemClick(account)}
        >
            <ListItemIcon>
                {getUserIcon(account.role)}
            </ListItemIcon>
            <ListItemText
                primary={account.name}
            />
            <ListItemSecondaryAction>
                <IconButton onClick={() => onItemResetClick(account)}>
                    <LockOpen/>
                </IconButton>
            </ListItemSecondaryAction>
        </ListItem>
    );
};

AccountListItem.propTypes = {
    account: PropTypes.object.isRequired,
    onItemClick: PropTypes.func.isRequired,
    onItemResetClick: PropTypes.func.isRequired,
};

export default AccountListItem; 