/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';
import ContentWrapper from '../shared/ContentWrapper';
import Snackbar from '@material-ui/core/Snackbar';
import { Card, CardContent, Typography } from '@material-ui/core';

const styles = {
    root: {
        flexGrow: 1,
    },
};

const showCount = 4;

class Events extends Component {
    state = {
        events: [],
        loadApi: {
            isExecuting: false,
            isErrored: false,
        },
        refreshApi: {
            isExecuting: false,
            isErrored: false,
        },
        snackbar: {
            message: '',
            open: false,
        },
        show: showCount
    }

    componentWillMount = () => {
        this.refresh('refreshApi');
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            api.get('/v1/events?offset=0&limit=100&orderBy=ASC')
            .then(response => {
                this.setState({ 
                    events: response.data,
                    [apiType]: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({ 
                    [apiType]: { isExecuting: false, isErrored: true },
                    snackbar: { message: error.response.data.Message, open: true },
                });
            });
        })
    }

    render() {
        let { events, loadApi, refreshApi, snackbar } = this.state;
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
                        <CardContent>
                            <Typography gutterBottom variant="headline" component="h2">
                                Events
                            </Typography>
                        </CardContent>
                    </Card>
                </ContentWrapper>
                <Snackbar
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'center'}}
                    open={snackbar.open}
                    onClose={this.handleSnackbarClose}
                    autoHideDuration={3000}
                    message={<span id="message-id">{snackbar.message}</span>}
                />
            </div>
        );
    }
}

Events.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Events); 