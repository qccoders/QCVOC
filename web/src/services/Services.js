/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withContext } from '../shared/ContextProvider';

import { withStyles } from '@material-ui/core/styles';

import ContentWrapper from '../shared/ContentWrapper';
import { Card, CardContent, Typography, CircularProgress, Button, ListSubheader } from '@material-ui/core';
import { Add } from '@material-ui/icons';
import ServiceList from './ServiceList';
import ServiceDialog from './ServiceDialog';
import { Shop, Work } from '@material-ui/icons'

const styles = {
    fab: {
        margin: 0,
        top: 'auto',
        right: 20,
        bottom: 20,
        left: 'auto',
        position: 'fixed',
        zIndex: 1000
    },
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
        marginTop: 72,
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
        serviceDialog: {
            open: false,
            intent: 'add',
            service: undefined,
        },
    }

    componentWillMount = () => {
        this.refresh('refreshApi');
    }

    handleAddClick = () => {
        this.setState({
            serviceDialog: {
                open: true,
                intent: 'add',
                service: undefined,
            }
        })
    }

    handleEditClick = (service) => {
        this.setState({
            serviceDialog: {
                open: true,
                intent: 'update',
                service: service,
            }
        })
    }

    handleServiceDialogClose = (result) => {
        this.setState({
            serviceDialog: {
                ...this.state.serviceDialog,
                open: false,
            }
        }, () => {
            if (!result) return;
            this.props.context.showMessage(result);
            this.refresh('refreshApi');
        })
    }

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true}}, () => {
            this.props.context.apiGet('/v1/services?offset=0&limit=5000&orderBy=ASC')
            .then(response => {
                this.setState({
                    services: response.data,
                    [apiType]: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({
                    [apiType]: { isExecuting: false, isErrored: true }
                });
            });
        })
    }

    render() {
        let { classes } = this.props;
        let { services, loadApi, refreshApi, serviceDialog } = this.state;

        let userDefined = services.filter(s => s.id !== '00000000-0000-0000-0000-000000000000');
        let systemDefined = services.filter(s => s.id === '00000000-0000-0000-0000-000000000000');

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
                                <div>
                                    {userDefined && userDefined.length > 0 && <ListSubheader>User Defined</ListSubheader>}
                                    <ServiceList
                                        services={userDefined}
                                        icon={<Shop/>}
                                        onItemClick={this.handleEditClick}
                                    />
                                    <ListSubheader>System Defined</ListSubheader>
                                    <ServiceList
                                        services={systemDefined}
                                        icon={<Work/>}
                                    />
                                </div>
                            }
                        </CardContent>
                    </Card>
                    <Button 
                        variant="fab" 
                        color="secondary" 
                        className={classes.fab}
                        onClick={this.handleAddClick}
                    >
                        <Add/>
                    </Button>
                    <ServiceDialog
                        open={serviceDialog.open}
                        intent={serviceDialog.intent}
                        onClose={this.handleServiceDialogClose}
                        service={serviceDialog.service}
                    />
                </ContentWrapper>
            </div>
        );
    }
}

Services.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(withContext(Services)); 