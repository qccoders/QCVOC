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
import { Card, CardContent, Typography, CircularProgress, Button, TextField, InputAdornment } from '@material-ui/core';
import { Add, Search } from '@material-ui/icons';
import VeteranList from './VeteranList';
import VeteranDialog from './VeteranDialog';

import { sortByProp } from '../util';

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
        marginTop: 83,
    },
    search: {
        width: '100%',
    },
    searchInput: {
        marginLeft: 25,
        marginRight: 25,
    }
};

const showCount = 7;

class Veterans extends Component {
    state = {
        veterans: [],
        loadApi: {
            isExecuting: false,
            isErrored: false,
        },
        refreshApi: {
            isExecuting: false,
            isErrored: false,
        },
        veteranDialog: {
            open: false,
            intent: 'add',
            veteran: undefined,
        },
        snackbar: {
            message: '',
            open: false,
        },
        filter: '',
        show: showCount,
    }

    componentWillMount = () => {
        this.refresh('refreshApi');
    }

    handleAddClick = () => {
        this.setState({
            veteranDialog: {
                open: true,
                intent: 'add',
                veteran: undefined,
            }
        })
    }
    
    handleEditClick = (veteran) => {
        this.setState({
            veteranDialog: {
                open: true,
                intent: 'update',
                veteran: veteran,
            }
        })
    }

    handleShowMoreClick = () => {
        this.setState({ show: this.state.show + showCount });
    }

    handleSearchChange = (event) => {
        this.setState({ 
            filter: event.target.value,
            show: showCount, 
        });
    }

    handleVeteranDialogClose = (result) => {
        this.setState({
            veteranDialog: {
                ...this.state.veteranDialog,
                open: false,
            }
        }, () => {
            if (!result) return;
            this.setState({ snackbar: { message: result, open: true }}, () => this.refresh('refreshApi'))
        })
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            api.get('/v1/veterans?offset=0&limit=5000&orderBy=ASC')
            .then(response => {
                this.setState({ 
                    veterans: response.data.map(p => ({ ...p, cardNumber: p.cardNumber || '' })),
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
        let { veterans, loadApi, refreshApi, snackbar, show, veteranDialog } = this.state;

        let searchById = this.state.filter !== undefined && this.state.filter !== '' && !isNaN(this.state.filter);

        let list = veterans
            .sort(sortByProp('firstName'))
            .filter(p => p.cardNumber.toString().includes(this.state.filter) || p.fullName.toLowerCase().includes(this.state.filter.toLowerCase()));

        let shownList = list.slice(0, this.state.show);

        return (
            <div>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
                        <CardContent>
                            <Typography gutterBottom variant="headline" component="h2">
                                Veterans
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
                                    style: styles.searchInput,
                                  }}
                            />
                            {refreshApi.isExecuting ? 
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <VeteranList
                                    veterans={shownList}
                                    displayId={searchById}
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
                    <VeteranDialog
                        open={veteranDialog.open}
                        intent={veteranDialog.intent} 
                        onClose={this.handleVeteranDialogClose}
                        veteran={veteranDialog.veteran}
                    />
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

Veterans.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Veterans); 