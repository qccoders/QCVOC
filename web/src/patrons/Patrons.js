import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';

import ContentWrapper from '../shared/ContentWrapper';
import Snackbar from '@material-ui/core/Snackbar';
import { Card, CardContent, Typography, CircularProgress, Button, TextField, InputAdornment } from '@material-ui/core';
import { Add, Search } from '@material-ui/icons';
import PatronList from './PatronList';
import PatronDialog from './PatronDialog';

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
        patronDialog: {
            open: false,
            intent: 'add',
            patron: undefined,
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

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            api.get('/v1/patrons?offset=0&limit=5000&orderBy=ASC')
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
        this.setState({
            patronDialog: {
                open: true,
                intent: 'add',
                patron: undefined,
            }
        })
    }

    handlePatronDialogClose = (result) => {
        this.setState({
            patronDialog: {
                ...this.state.patronDialog,
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

    handleEditClick = (patron) => {
        this.setState({
            patronDialog: {
                open: true,
                intent: 'update',
                patron: patron,
            }
        })
    }

    handleSearchChange = (event) => {
        this.setState({ 
            filter: event.target.value,
            show: showCount, 
        });
    }

    handleShowMoreClick = () => {
        this.setState({ show: this.state.show + showCount });
    }

    render() {
        let { classes } = this.props;
        let { patrons, loadApi, refreshApi, snackbar, show, patronDialog } = this.state;

        let list = patrons
            .sort(sortByProp('firstName'))
            .filter(p => p.fullName.toLowerCase().includes(this.state.filter.toLowerCase()));

        let shownList = list.slice(0, this.state.show);

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
                                    style: styles.searchInput,
                                  }}
                            />
                            {refreshApi.isExecuting ? 
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <PatronList
                                    patrons={shownList}
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
                    <PatronDialog
                        open={patronDialog.open}
                        intent={patronDialog.intent} 
                        onClose={this.handlePatronDialogClose}
                        patron={patronDialog.patron}
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

Patrons.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Patrons); 