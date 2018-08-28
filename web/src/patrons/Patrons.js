import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';

import ContentWrapper from '../shared/ContentWrapper';
import Snackbar from '@material-ui/core/Snackbar';
import { Card, CardContent, Typography, CircularProgress, Button } from '@material-ui/core';
import { Add } from '@material-ui/icons';

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
        minHeight: 125,
    },
    refreshSpinner: {
        position: 'fixed',
        left: 0,
        right: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 25,
    },
};

class Patrons extends Component {
    state = {
        patrons: [],
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
        this.refresh('loadApi');
    }

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            api.get('/v1/patrons')
            .then(response => {
                this.setState({ 
                    patrons: response.data,
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

    handleAddClick = () => {

    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    render() {
        let { classes } = this.props;
        let { loadApi, refreshApi, snackbar } = this.state;

        return (
            <div>
                <ContentWrapper api={loadApi}>
                    <Card>
                        <CardContent className={classes.card}>
                            <Typography gutterBottom variant="headline" componet="h2">
                                Patrons
                            </Typography>
                            {refreshApi.isExecuting ? 
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <p>Patron list goes here</p>
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

Patrons.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Patrons); 