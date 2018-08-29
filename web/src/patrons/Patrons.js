import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';

import ContentWrapper from '../shared/ContentWrapper';
import Snackbar from '@material-ui/core/Snackbar';
import { Card, CardContent, Typography, CircularProgress, Button, Paper, TextField, Divider, InputAdornment } from '@material-ui/core';
import { Add, Search } from '@material-ui/icons';
import PatronList from './PatronList';

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
        maxWidth: 800,
        margin: 'auto',
    },
    refreshSpinner: {
        position: 'fixed',
        left: 0,
        right: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 25,
    },
    search: {
        width: '100%'
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
        filter: '',
        show: 10,
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

    handleEditClick = () => {

    }

    handleSearchChange = (event) => {
        this.setState({ filter: event.target.value });
    }

    handleShowMoreClick = () => {
        this.setState({ show: this.state.show + 10 });
    }

    render() {
        let { classes } = this.props;
        let { patrons, loadApi, refreshApi, snackbar, show } = this.state;

        let list = patrons
            .filter(p => p.fullName.toLowerCase().includes(this.state.filter.toLowerCase()));

        return (
            <div>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
                        <CardContent>
                            <Typography gutterBottom variant="headline" componet="h2">
                                Patrons
                            </Typography>
                            <TextField
                                type="search"
                                className={classes.search}
                                margin="normal"
                                onChange={this.handleSearchChange}
                                InputProps={{
                                    startAdornment: (
                                      <InputAdornment position="start">
                                        <Search />
                                      </InputAdornment>
                                    ),
                                  }}
                            />
                            {refreshApi.isExecuting ? 
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <PatronList
                                    patrons={list}
                                    onItemClick={this.handleEditClick}
                                />
                            }
                            {list.length > show && <Button fullWidth onClick={this.handleShowMoreClick}>Show More</Button>}
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