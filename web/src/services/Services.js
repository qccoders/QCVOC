/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';

import ContentWrapper from '../shared/ContentWrapper';
import { Card, CardContent, Typography, CircularProgress } from '@material-ui/core';
import ServiceList from './ServiceList';

const styles = {
    card: {
        minHeight: 220,
        maxWidth: 800,
        margin: 'auto',
    },
    refreshSpinner: {
        position: 'fixed',
        left: 0,
        right: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 83,
    },
};

class Services extends Component {
    state = {
        services: [],
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
    }

    componentWillMount = () => {
        this.refresh('refreshApi');
    }

    handleEditClick = (service) => {

    }

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true}}, () => {
            api.get('/v1/services?offset=0&limit=5000&orderBy=ASC')
            .then(response => {
                this.setState({
                    services: response.data,
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
        let { classes } = this.props;
        let { services, loadApi, refreshApi, snackbar } = this.state;

        return (
            <div>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
                        <CardContent>
                            <Typography gutterBottom variant="headline" component="h2">
                                Services
                            </Typography>
                            {refreshApi.isExecuting ?
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <ServiceList
                                    services={services}
                                    onItemClick={this.handleEditClick}
                                />
                            }
                        </CardContent>
                    </Card>
                </ContentWrapper>
            </div>
        );
    }
}

Services.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Services); 