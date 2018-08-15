import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import ContentWrapper from '../shared/ContentWrapper';
import { withStyles } from '@material-ui/core/styles';
import { Typography, List, ListItem, ListItemIcon, ListItemText, Card, CardContent } from '@material-ui/core';
import { Person, Star } from '@material-ui/icons'

const styles = {
};

class Accounts extends Component {
    state = { 
        accounts: [],
        api: {
            isExecuting: false,
            isErrored: false,
        },
    };

    componentWillMount = () => {
        this.refresh();
    }

    refresh = () => {
        this.setState({ ...this.state, api: { ...this.state.api, isExecuting: true }}, () => {
            api.get('/v1/accounts')
            .then(response => {
                this.setState({ 
                    accounts: response.data,
                    api: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({ ...this.state, api: { isExecuting: false, isErrored: true }});
                console.log(error);
            });
        })
    }

    render() {
        let { accounts, api } = this.state;

        return (
            <ContentWrapper api={api}>
                <Card>
                    <CardContent>
                        <Typography gutterBottom variant="headline" component="h2">
                            Accounts
                        </Typography>
                        <List>
                            {accounts.map(a => 
                                <ListItem key={a.id}>
                                    <ListItemIcon>
                                        {a.role === 'Administrator' ? <Star/> : <Person/>}
                                    </ListItemIcon>
                                    <ListItemText
                                        primary={a.name}
                                        secondary={a.role}
                                    />
                                </ListItem>
                            )}
                        </List>
                    </CardContent>
                </Card>
            </ContentWrapper>
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 