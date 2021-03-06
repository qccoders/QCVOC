/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { 
    Card, 
    CardContent, 
    Typography, 
    CircularProgress, 
    Button, 
    TextField, 
    InputAdornment,
    Fab,
} from '@material-ui/core';
import { Add, Search } from '@material-ui/icons';

import { withContext } from '../shared/ContextProvider';
import { sortByProp } from '../util';
import ContentWrapper from '../shared/ContentWrapper';
import VeteranList from './VeteranList';
import VeteranDialog from './VeteranDialog';

const styles = {
    fab: {
        margin: 0,
        top: 'auto',
        right: 20,
        bottom: 20,
        left: 'auto',
        position: 'fixed',
        zIndex: 1000,
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
    },
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
            },
        });
    }
    
    handleEditClick = (veteran) => {
        this.setState({
            veteranDialog: {
                open: true,
                intent: 'update',
                veteran: veteran,
            },
        });
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
            },
        }, () => {
            if (!result) return;
            this.props.context.showMessage(result);
            this.refresh('refreshApi');
        });
    }

    refresh = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            this.props.context.api.get('/v1/veterans?offset=0&limit=5000&orderBy=ASC')
            .then(response => {
                this.setState({ 
                    veterans: response.data.map(p => ({ ...p, cardNumber: p.cardNumber || '' })),
                    [apiType]: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({ 
                    [apiType]: { isExecuting: false, isErrored: true },
                });
            });
        });
    }

    render() {
        let { classes } = this.props;
        let { veterans, loadApi, refreshApi, show, veteranDialog } = this.state;

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
                            <Typography gutterBottom variant="h5">
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
                                <div>
                                    <VeteranList
                                        veterans={shownList}
                                        displayId={searchById}
                                        onItemClick={this.handleEditClick}
                                    />
                                    {list.length > show && <Button fullWidth onClick={this.handleShowMoreClick}>Show More</Button>}
                                </div>
                            }
                        </CardContent>
                    </Card>
                    <Fab
                        color="secondary" 
                        className={classes.fab}
                        onClick={this.handleAddClick}
                    >
                        <Add/>
                    </Fab>
                    <VeteranDialog
                        open={veteranDialog.open}
                        intent={veteranDialog.intent} 
                        onClose={this.handleVeteranDialogClose}
                        veteran={veteranDialog.veteran}
                    />
                </ContentWrapper>
            </div>
        );
    }
}

Veterans.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(withContext(Veterans)); 