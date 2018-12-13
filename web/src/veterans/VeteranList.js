/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import PropTypes from 'prop-types';

import { List, ListItem, ListItemIcon, ListItemText } from '@material-ui/core';
import { Person } from '@material-ui/icons';

import { sortByProp } from '../util';

const VeteranList = (props) => {
    let { veterans, displayId, onItemClick } = props;

    return (
        <List>
            {veterans.sort(sortByProp('fullName')).map(p => 
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
                        secondary={displayId ? 'Card Number: ' + p.cardNumber : p.address}
                    />
                </ListItem>
            )}
            {veterans.length === 0 && <ListItem><ListItemText primary='No Veterans have been added.'/></ListItem>}
        </List>
    );
};

VeteranList.propTypes = {
    veterans: PropTypes.array.isRequired,
    displayId: PropTypes.bool.isRequired,
    onItemClick: PropTypes.func.isRequired,
};

export default VeteranList; 